using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MKPay.Core.DTOs.Common;
using MKPay.Core.DTOs.Transaction;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Exceptions;
using MKPay.Core.Interfaces;
using MKPay.Core.Interfaces.Services;
using MKPay.Infrastructure.Data;
using MKPay.Infrastructure.Utils;

namespace MKPay.Infrastructure.Services;

public class TransactionService : ITransactionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public TransactionService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _context = context;
        _emailService = emailService;
    }

    public async Task<TransactionResponseDto> SendMoneyAsync(Guid senderId, SendMoneyRequestDto request)
    {
        TransactionResponseDto result = null!;

        var strategy = _context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Get sender's account
                var senderAccount = await _unitOfWork.Accounts.GetByUserIdAsync(senderId);
                if (senderAccount == null)
                    throw new MKPayException("Sender account not found");

                // 2. Find receiver by email
                var receiver = await _userManager.FindByEmailAsync(request.ReceiverEmail);
                if (receiver == null)
                    throw new UserNotFoundException(request.ReceiverEmail);

                // 3. Get receiver's account
                var receiverAccount = await _unitOfWork.Accounts.GetByUserIdAsync(receiver.Id);
                if (receiverAccount == null)
                    throw new MKPayException("Receiver account not found");

                // 4. Validate sender cannot send to themselves
                if (senderAccount.Id == receiverAccount.Id)
                    throw new InvalidTransactionException("Cannot send money to yourself");

                // 5. Check sufficient balance
                if (!await _unitOfWork.Accounts.HasSufficientBalanceAsync(senderAccount.Id, request.Amount))
                    throw new InsufficientBalanceException(request.Amount, senderAccount.Balance);

                // 6. Check both accounts are active
                if (!senderAccount.IsActive || !receiverAccount.IsActive)
                    throw new MKPayException("One or both accounts are not active");

                // 7. Create transaction
                var transaction = new Transaction
                {
                    SenderAccountId = senderAccount.Id,
                    ReceiverAccountId = receiverAccount.Id,
                    Amount = request.Amount,
                    Currency = Enum.Parse<Currency>(request.Currency),
                    Status = TransactionStatus.Pending,
                    Type = TransactionType.Transfer,
                    Description = request.Description,
                    ReferenceNumber = TransactionReferenceGenerator.Generate(),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Transactions.AddAsync(transaction);

                // 8. Update balances
                senderAccount.Balance -= request.Amount;
                senderAccount.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Accounts.UpdateAsync(senderAccount);

                receiverAccount.Balance += request.Amount;
                receiverAccount.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Accounts.UpdateAsync(receiverAccount);

                // 9. Mark transaction as completed
                transaction.Status = TransactionStatus.Completed;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Transactions.UpdateAsync(transaction);

                // 10. Log the transaction
                await _unitOfWork.AuditLogs.LogActionAsync(
                    senderId,
                    "MoneyTransfer",
                    "Transaction",
                    transaction.Id,
                    $"Transferred {request.Amount} {request.Currency} to {request.ReceiverEmail}",
                    "System",
                    "System"
                );

                // 11. Commit
                await _unitOfWork.CommitTransactionAsync();

                var sender = await _userManager.FindByIdAsync(senderId.ToString());
                result = new TransactionResponseDto
                {
                    Id = transaction.Id,
                    ReferenceNumber = transaction.ReferenceNumber!,
                    Amount = transaction.Amount,
                    Currency = transaction.Currency.ToString(),
                    Status = transaction.Status.ToString(),
                    Type = transaction.Type.ToString(),
                    Description = transaction.Description,
                    CreatedAt = transaction.CreatedAt,
                    SenderName = $"{sender!.FirstName} {sender.LastName}",
                    SenderEmail = sender.Email!,
                    ReceiverName = $"{receiver.FirstName} {receiver.LastName}",
                    ReceiverEmail = receiver.Email!
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        });

        // 12. Send email notifications outside the DB transaction
        await _emailService.SendTransactionNotificationAsync(
            result.SenderEmail,
            result.SenderName,
            result.Amount,
            "Debit"
        );
        await _emailService.SendTransactionNotificationAsync(
            result.ReceiverEmail,
            result.ReceiverName,
            result.Amount,
            "Credit"
        );

        return result;
    }

    public async Task<TransactionResponseDto?> GetTransactionByIdAsync(Guid transactionId)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
        
        if (transaction == null)
            return null;

        return await MapToDtoAsync(transaction);
    }

    public async Task<TransactionResponseDto?> GetTransactionByReferenceAsync(string referenceNumber)
    {
        var transaction = await _unitOfWork.Transactions.GetByReferenceNumberAsync(referenceNumber);
        
        if (transaction == null)
            return null;

        return await MapToDtoAsync(transaction);
    }

    public async Task<PaginatedResponse<TransactionResponseDto>> GetUserTransactionsAsync(
        Guid userId, 
        int pageNumber = 1, 
        int pageSize = 20)
    {
        var skip = (pageNumber - 1) * pageSize;
        var transactions = await _unitOfWork.Transactions.GetUserTransactionsAsync(userId, skip, pageSize);
        var totalCount = await _unitOfWork.Transactions.CountAsync();

        var transactionDtos = new List<TransactionResponseDto>();
        foreach (var transaction in transactions)
        {
            transactionDtos.Add(await MapToDtoAsync(transaction));
        }

        return new PaginatedResponse<TransactionResponseDto>
        {
            Items = transactionDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<TransactionResponseDto>> GetRecentTransactionsAsync(Guid userId, int count = 10)
    {
        var transactions = await _unitOfWork.Transactions.GetUserTransactionsAsync(userId, 0, count);
        
        var transactionDtos = new List<TransactionResponseDto>();
        foreach (var transaction in transactions)
        {
            transactionDtos.Add(await MapToDtoAsync(transaction));
        }

        return transactionDtos;
    }

    public async Task<bool> CancelTransactionAsync(Guid transactionId, Guid userId)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(transactionId);
        
        if (transaction == null)
            throw new TransactionNotFoundException(transactionId);

        // Get user's account to verify ownership
        var userAccount = await _unitOfWork.Accounts.GetByUserIdAsync(userId);
        if (userAccount == null || transaction.SenderAccountId != userAccount.Id)
        {
            throw new UnauthorizedOperationException("You can only accept payment requests sent to you");        }

        // Can only cancel pending transactions
        if (transaction.Status != TransactionStatus.Pending)
        {
            throw new InvalidTransactionException("Can only cancel pending transactions");
        }

        transaction.Status = TransactionStatus.Cancelled;
        transaction.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.Transactions.UpdateAsync(transaction);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task<TransactionResponseDto> MapToDtoAsync(Transaction transaction)
    {
        // Load related data if not already loaded
        var senderAccount = transaction.SenderAccount ?? 
            await _unitOfWork.Accounts.GetByIdAsync(transaction.SenderAccountId);
        var receiverAccount = transaction.ReceiverAccount ?? 
            await _unitOfWork.Accounts.GetByIdAsync(transaction.ReceiverAccountId);

        var sender = senderAccount?.User ?? 
            await _userManager.FindByIdAsync(senderAccount!.UserId.ToString());
        var receiver = receiverAccount?.User ?? 
            await _userManager.FindByIdAsync(receiverAccount!.UserId.ToString());

        return new TransactionResponseDto
        {
            Id = transaction.Id,
            ReferenceNumber = transaction.ReferenceNumber ?? string.Empty,
            Amount = transaction.Amount,
            Currency = transaction.Currency.ToString(),
            Status = transaction.Status.ToString(),
            Type = transaction.Type.ToString(),
            Description = transaction.Description,
            CreatedAt = transaction.CreatedAt,
            SenderName = sender != null ? $"{sender.FirstName} {sender.LastName}" : "Unknown",
            SenderEmail = sender?.Email ?? "Unknown",
            ReceiverName = receiver != null ? $"{receiver.FirstName} {receiver.LastName}" : "Unknown",
            ReceiverEmail = receiver?.Email ?? "Unknown"
        };
    }
}