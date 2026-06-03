namespace MKPay.Core.Entities;

public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // Login, Transaction, etc.
    public string EntityType { get; set; } = string.Empty; // Transaction, User, etc.
    public Guid? EntityId { get; set; }
    public string Details { get; set; } = string.Empty; // JSON details
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
}