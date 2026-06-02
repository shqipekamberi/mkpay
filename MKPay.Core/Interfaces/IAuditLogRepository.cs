using MKPay.Core.Entities;

namespace MKPay.Core.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetUserAuditLogsAsync(Guid userId, int skip = 0, int take = 100);
    Task<IEnumerable<AuditLog>> GetAuditLogsByActionAsync(string action, int skip = 0, int take = 100);
    Task<IEnumerable<AuditLog>> GetRecentAuditLogsAsync(int count = 50);
    Task LogActionAsync(Guid? userId, string action, string entityType, Guid? entityId, string details, string ipAddress, string userAgent);
}