using System.ComponentModel.DataAnnotations;

namespace MKPay.Core.DTOs.Transaction;

public class SendMoneyRequestDto
{
    [Required]
    public string ReceiverEmail { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Currency { get; set; } = "MKD"; // Default to Macedonian Denar
}