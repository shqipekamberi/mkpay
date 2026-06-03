using Microsoft.EntityFrameworkCore.Storage;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    // Repository instances
    private IAccountRepository? _accounts;
    private ITransactionRepository? _transactions;
    private IPaymentRequestRepository? _paymentRequests;
    private IBillSplitRepository? _billSplits;
    private IBillSplitParticipantRepository? _billSplitParticipants;
    private IAuditLogRepository? _auditLogs;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Lazy initialization of repositories
    public IAccountRepository Accounts => 
        _accounts ??= new AccountRepository(_context);

    public ITransactionRepository Transactions => 
        _transactions ??= new TransactionRepository(_context);

    public IPaymentRequestRepository PaymentRequests => 
        _paymentRequests ??= new PaymentRequestRepository(_context);

    public IBillSplitRepository BillSplits => 
        _billSplits ??= new BillSplitRepository(_context);

    public IBillSplitParticipantRepository BillSplitParticipants => 
        _billSplitParticipants ??= new BillSplitParticipantRepository(_context);

    public IAuditLogRepository AuditLogs => 
        _auditLogs ??= new AuditLogRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}