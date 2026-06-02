namespace MKPay.Core.Entities;

public class BillSplitParticipant : BaseEntity
{
    public Guid BillSplitId { get; set; }
    public Guid UserId { get; set; }
    public decimal AmountOwed { get; set; }
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidAt { get; set; }
    public Guid? TransactionId { get; set; } // Link to payment transaction
    
    // Navigation properties
    public virtual BillSplit BillSplit { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}