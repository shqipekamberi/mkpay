using MKPay.Core.Enums;

namespace MKPay.Core.Entities;

public class Account : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Balance { get; set; } = 0;
    public Currency Currency { get; set; } = Currency.MKD;
    public bool IsActive { get; set; } = true;
    public string AccountNumber { get; set; } = string.Empty; // Generated unique number
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = null!;
}