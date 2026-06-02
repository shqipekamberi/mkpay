using MKPay.Core.Enums;

namespace MKPay.Core.Entities;

public class Transaction : BaseEntity
{
    public Guid SenderAccountId { get; set; }
    public Guid ReceiverAccountId { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public TransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; } // Unique transaction reference
    
    // For bill splits
    public Guid? BillSplitId { get; set; }
    
    // For payment requests
    public Guid? PaymentRequestId { get; set; }
    
    // Navigation properties
    public virtual Account SenderAccount { get; set; } = null!;
    public virtual Account ReceiverAccount { get; set; } = null!;
    public virtual BillSplit? BillSplit { get; set; }
    public virtual PaymentRequest? PaymentRequest { get; set; }
}