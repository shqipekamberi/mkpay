using MKPay.Core.Entities;
using MKPay.Core.Enums;

namespace MKPay.Core.Interfaces;

public interface ITransactionRepository : IRepository<Transaction>
{
    Task<IEnumerable<Transaction>> GetUserTransactionsAsync(Guid userId, int skip = 0, int take = 50);
    Task<IEnumerable<Transaction>> GetAccountTransactionsAsync(Guid accountId, int skip = 0, int take = 50);
    Task<Transaction?> GetByReferenceNumberAsync(string referenceNumber);
    Task<IEnumerable<Transaction>> GetTransactionsByStatusAsync(TransactionStatus status);
    Task<IEnumerable<Transaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? userId = null);
    Task<decimal> GetTotalSentAmountAsync(Guid userId, DateTime? fromDate = null);
    Task<decimal> GetTotalReceivedAmountAsync(Guid userId, DateTime? fromDate = null);
}