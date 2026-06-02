using System.ComponentModel.DataAnnotations;

namespace MKPay.Core.DTOs.BillSplit;

public class CreateBillSplitDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "MKD";

    [Required]
    [MinLength(1)]
    public List<BillSplitParticipantDto> Participants { get; set; } = new();
}

public class BillSplitParticipantDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal AmountOwed { get; set; }
}