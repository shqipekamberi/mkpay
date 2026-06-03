using MKPay.Core.DTOs.Common;
using MKPay.Core.DTOs.Transaction;

namespace MKPay.Core.Interfaces.Services;

public interface ITransactionService
{
    Task<TransactionResponseDto> SendMoneyAsync(Guid senderId, SendMoneyRequestDto request);
    Task<TransactionResponseDto?> GetTransactionByIdAsync(Guid transactionId);
    Task<TransactionResponseDto?> GetTransactionByReferenceAsync(string referenceNumber);
    Task<PaginatedResponse<TransactionResponseDto>> GetUserTransactionsAsync(Guid userId, int pageNumber = 1, int pageSize = 20);
    Task<IEnumerable<TransactionResponseDto>> GetRecentTransactionsAsync(Guid userId, int count = 10);
    Task<bool> CancelTransactionAsync(Guid transactionId, Guid userId);
}