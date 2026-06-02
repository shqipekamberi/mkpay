using Microsoft.AspNetCore.Identity;

namespace MKPay.Core.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    // Extended properties beyond Identity defaults
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual Account? Account { get; set; }
    public virtual ICollection<Transaction> SentTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Transaction> ReceivedTransactions { get; set; } = new List<Transaction>();
    public virtual ICollection<PaymentRequest> CreatedPaymentRequests { get; set; } = new List<PaymentRequest>();
    public virtual ICollection<PaymentRequest> ReceivedPaymentRequests { get; set; } = new List<PaymentRequest>();
}