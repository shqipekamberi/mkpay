using Microsoft.EntityFrameworkCore;
using MKPay.Core.Entities;
using MKPay.Core.Interfaces;
using MKPay.Infrastructure.Data;

namespace MKPay.Infrastructure.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(Guid userId, int skip = 0, int take = 100)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action, int skip = 0, int take = 100)
    {
        return await _dbSet
            .Include(al => al.User)
            .Where(al => al.Action == action)
            .OrderByDescending(al => al.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 50)
    {
        return await _dbSet
            .Include(al => al.User)
            .OrderByDescending(al => al.CreatedAt)
            .Take(count)
            .ToListAsync();
    }

    public async Task LogActionAsync(
        Guid? userId, 
        string action, 
        string entityType, 
        Guid? entityId, 
        string details, 
        string ipAddress, 
        string userAgent)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Details = details,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await AddAsync(auditLog);
    }
}