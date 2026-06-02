using MKPay.Core.Enums;

namespace MKPay.Core.Entities;

public class BillSplit : BaseEntity
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public Currency Currency { get; set; }
    public bool IsSettled { get; set; } = false; // All participants paid
    public DateTime? SettledAt { get; set; }
    
    // Navigation properties
    public virtual ApplicationUser Creator { get; set; } = null!;
    public virtual ICollection<BillSplitParticipant> Participants { get; set; } = new List<BillSplitParticipant>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}