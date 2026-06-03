using MKPay.Core.Enums;

namespace MKPay.Core.Entities;

public class PaymentRequest : BaseEntity
{
    public Guid RequesterId { get; set; } // Who is requesting money
    public Guid RequesteeId { get; set; } // Who needs to pay
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public PaymentRequestStatus Status { get; set; } = PaymentRequestStatus.Pending;
    public string Description { get; set; } = string.Empty;
    public DateTime? RespondedAt { get; set; }
    public DateTime ExpiresAt { get; set; } // Request expires after X days
    
    // Navigation properties
    public virtual ApplicationUser Requester { get; set; } = null!;
    public virtual ApplicationUser Requestee { get; set; } = null!;
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}