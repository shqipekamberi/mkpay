namespace MKPay.Core.DTOs.BillSplit;

public class BillSplitResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public bool IsSettled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? SettledAt { get; set; }
    
    public string CreatorName { get; set; } = string.Empty;
    public string CreatorEmail { get; set; } = string.Empty;
    
    public List<ParticipantResponseDto> Participants { get; set; } = new();
}

public class ParticipantResponseDto
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal AmountOwed { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
}