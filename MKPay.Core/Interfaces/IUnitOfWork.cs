namespace MKPay.Core.Interfaces;

/// <summary>
/// Unit of Work pattern to manage database transactions
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IAccountRepository Accounts { get; }
    ITransactionRepository Transactions { get; }
    IPaymentRequestRepository PaymentRequests { get; }
    IBillSplitRepository BillSplits { get; }
    IBillSplitParticipantRepository BillSplitParticipants { get; }
    IAuditLogRepository AuditLogs { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}