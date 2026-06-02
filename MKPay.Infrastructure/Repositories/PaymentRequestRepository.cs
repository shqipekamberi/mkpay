using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Enums;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class PaymentRequestRepository : Repository<PaymentRequest>, IPaymentRequestRepository
{
    public PaymentRequestRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<PaymentRequest>> GetUserCreatedRequestsAsync(Guid userId)
    {
        return await _dbSet
            .Include(pr => pr.Requester)
            .Include(pr => pr.Requestee)
            .Where(pr => pr.RequesterId == userId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentRequest>> GetUserReceivedRequestsAsync(Guid userId)
    {
        return await _dbSet
            .Include(pr => pr.Requester)
            .Include(pr => pr.Requestee)
            .Where(pr => pr.RequesteeId == userId)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentRequest>> GetPendingRequestsAsync(Guid userId)
    {
        return await _dbSet
            .Include(pr => pr.Requester)
            .Include(pr => pr.Requestee)
            .Where(pr => pr.RequesteeId == userId && pr.Status == PaymentRequestStatus.Pending)
            .OrderByDescending(pr => pr.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<PaymentRequest>> GetExpiredRequestsAsync()
    {
        return await _dbSet
            .Where(pr => pr.Status == PaymentRequestStatus.Pending && pr.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task<int> GetPendingRequestsCountAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(pr => pr.RequesteeId == userId && pr.Status == PaymentRequestStatus.Pending);
    }
}