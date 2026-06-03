using Microsoft.AspNetCore.Identity;
using MKPay.Core.DTOs.BillSplit;
using MKPay.Core.DTOs.Transaction;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Exceptions;
using MKPay.Core.Interfaces;
using MKPay.Core.Interfaces.Services;

namespace MKPay.Infrastructure.Services;
public class BillSplitService : IBillSplitService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITransactionService _transactionService;

    public BillSplitService(
        IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager,
        ITransactionService transactionService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _transactionService = transactionService;
    }

    public async Task<BillSplitResponseDto> CreateBillSplitAsync(Guid creatorId, CreateBillSplitDto request)
    {
        // Validate participants exist
        var participantUsers = new List<ApplicationUser>();
        foreach (var participant in request.Participants)
        {
            var user = await _userManager.FindByEmailAsync(participant.Email);
            if (user == null)
            {
                throw new UserNotFoundException(participant.Email);
            }
            participantUsers.Add(user);
        }

        // Validate total amounts match
        var totalParticipantAmounts = request.Participants.Sum(p => p.AmountOwed);
        if (Math.Abs(totalParticipantAmounts - request.TotalAmount) > 0.01m) // Allow small rounding difference
        {
            throw new InvalidTransactionException(
                $"Participant amounts ({totalParticipantAmounts}) don't match total amount ({request.TotalAmount})");
        }

        // Create bill split
        var billSplit = new BillSplit
        {
            CreatorId = creatorId,
            Title = request.Title,
            Description = request.Description,
            TotalAmount = request.TotalAmount,
            Currency = Enum.Parse<Currency>(request.Currency),
            IsSettled = false,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.BillSplits.AddAsync(billSplit);
        await _unitOfWork.SaveChangesAsync();

        // Create participants
        for (int i = 0; i < request.Participants.Count; i++)
        {
            var participantDto = request.Participants[i];
            var participantUser = participantUsers[i];

            var participant = new BillSplitParticipant
            {
                BillSplitId = billSplit.Id,
                UserId = participantUser.Id,
                AmountOwed = participantDto.AmountOwed,
                IsPaid = participantUser.Id == creatorId, // Creator is automatically marked as paid
                PaidAt = participantUser.Id == creatorId ? DateTime.UtcNow : null,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.BillSplitParticipants.AddAsync(participant);
        }

        await _unitOfWork.SaveChangesAsync();

        // Check if all participants are already paid (creator-only scenario)
        var allParticipants = await _unitOfWork.BillSplitParticipants.GetParticipantsByBillSplitIdAsync(billSplit.Id);
        if (allParticipants.All(p => p.IsPaid))
        {
            billSplit.IsSettled = true;
            billSplit.SettledAt = DateTime.UtcNow;
            await _unitOfWork.BillSplits.UpdateAsync(billSplit);
            await _unitOfWork.SaveChangesAsync();
        }

        // Log the action
        await _unitOfWork.AuditLogs.LogActionAsync(
            creatorId,
            "BillSplitCreated",
            "BillSplit",
            billSplit.Id,
            $"Created bill split '{request.Title}' for {request.TotalAmount} {request.Currency}",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        return await MapToDtoAsync(billSplit);
    }

    public async Task<BillSplitResponseDto?> GetBillSplitByIdAsync(Guid billSplitId, Guid userId)
    {
        // Check if user has access to this bill split
        if (!await CanUserAccessBillSplitAsync(billSplitId, userId))
        {
            throw new UnauthorizedOperationException("You don't have access to this bill split");
        }

        var billSplit = await _unitOfWork.BillSplits.GetBillSplitWithParticipantsAsync(billSplitId);
        
        if (billSplit == null)
            return null;

        return await MapToDtoAsync(billSplit);
    }

    public async Task<IEnumerable<BillSplitResponseDto>> GetUserBillSplitsAsync(Guid userId)
    {
        var billSplits = await _unitOfWork.BillSplits.GetUserBillSplitsAsync(userId);
        
        var dtos = new List<BillSplitResponseDto>();
        foreach (var billSplit in billSplits)
        {
            dtos.Add(await MapToDtoAsync(billSplit));
        }

        return dtos;
    }

    public async Task<IEnumerable<BillSplitResponseDto>> GetUnsettledBillSplitsAsync(Guid userId)
    {
        var billSplits = await _unitOfWork.BillSplits.GetUnsettledBillSplitsAsync(userId);
        
        var dtos = new List<BillSplitResponseDto>();
        foreach (var billSplit in billSplits)
        {
            dtos.Add(await MapToDtoAsync(billSplit));
        }

        return dtos;
    }

    public async Task<BillSplitResponseDto> PayBillSplitShareAsync(Guid billSplitId, Guid userId)
    {
        // Get bill split with participants
        var billSplit = await _unitOfWork.BillSplits.GetBillSplitWithParticipantsAsync(billSplitId);
        
        if (billSplit == null)
        {
            throw new MKPayException("Bill split not found");
        }

        // Check if already settled
        if (billSplit.IsSettled)
        {
            throw new InvalidTransactionException("Bill split is already settled");
        }

        // Get participant record for this user
        var participant = await _unitOfWork.BillSplitParticipants.GetParticipantAsync(billSplitId, userId);
        
        if (participant == null)
        {
            throw new MKPayException("You are not a participant in this bill split");
        }

        // Check if already paid
        if (participant.IsPaid)
        {
            throw new InvalidTransactionException("You have already paid your share");
        }

        // Process payment to creator
        var creator = await _userManager.FindByIdAsync(billSplit.CreatorId.ToString());
        if (creator == null)
        {
            throw new UserNotFoundException("Bill split creator not found");
        }

        var sendMoneyRequest = new SendMoneyRequestDto
        {
            ReceiverEmail = creator.Email!,
            Amount = participant.AmountOwed,
            Description = $"Payment for bill split: {billSplit.Title}",
            Currency = billSplit.Currency.ToString()
        };

        var transaction = await _transactionService.SendMoneyAsync(userId, sendMoneyRequest);

        // Mark participant as paid
        await _unitOfWork.BillSplitParticipants.MarkAsPaidAsync(participant.Id, Guid.Parse(transaction.Id.ToString()));

        // Check if all participants have paid
        var unpaidParticipants = await _unitOfWork.BillSplitParticipants.GetUnpaidParticipantsAsync(billSplitId);
        if (!unpaidParticipants.Any())
        {
            billSplit.IsSettled = true;
            billSplit.SettledAt = DateTime.UtcNow;
            billSplit.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.BillSplits.UpdateAsync(billSplit);
        }

        await _unitOfWork.SaveChangesAsync();

        // Log the action
        await _unitOfWork.AuditLogs.LogActionAsync(
            userId,
            "BillSplitPayment",
            "BillSplit",
            billSplit.Id,
            $"Paid {participant.AmountOwed} {billSplit.Currency} for bill split '{billSplit.Title}'",
            "System",
            "System"
        );
        await _unitOfWork.SaveChangesAsync();

        // Return updated bill split
        return await MapToDtoAsync(billSplit);
    }

    public async Task<bool> CanUserAccessBillSplitAsync(Guid billSplitId, Guid userId)
    {
        return await _unitOfWork.BillSplits.IsUserParticipantAsync(billSplitId, userId);
    }

    private async Task<BillSplitResponseDto> MapToDtoAsync(BillSplit billSplit)
    {
        var creator = billSplit.Creator ?? 
            await _userManager.FindByIdAsync(billSplit.CreatorId.ToString());

        // Get participants if not loaded
        var participants = billSplit.Participants.Any() 
            ? billSplit.Participants.ToList()
            : (await _unitOfWork.BillSplitParticipants.GetParticipantsByBillSplitIdAsync(billSplit.Id)).ToList();

        var participantDtos = new List<ParticipantResponseDto>();
        foreach (var participant in participants)
        {
            var user = participant.User ?? 
                await _userManager.FindByIdAsync(participant.UserId.ToString());

            participantDtos.Add(new ParticipantResponseDto
            {
                UserId = participant.UserId,
                Name = user != null ? $"{user.FirstName} {user.LastName}" : "Unknown",
                Email = user?.Email ?? "Unknown",
                AmountOwed = participant.AmountOwed,
                IsPaid = participant.IsPaid,
                PaidAt = participant.PaidAt
            });
        }

        return new BillSplitResponseDto
        {
            Id = billSplit.Id,
            Title = billSplit.Title,
            Description = billSplit.Description,
            TotalAmount = billSplit.TotalAmount,
            Currency = billSplit.Currency.ToString(),
            IsSettled = billSplit.IsSettled,
            CreatedAt = billSplit.CreatedAt,
            SettledAt = billSplit.SettledAt,
            CreatorName = creator != null ? $"{creator.FirstName} {creator.LastName}" : "Unknown",
            CreatorEmail = creator?.Email ?? "Unknown",
            Participants = participantDtos
        };
    }
}