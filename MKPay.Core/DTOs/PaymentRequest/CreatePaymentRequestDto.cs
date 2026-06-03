using System.ComponentModel.DataAnnotations;

namespace MKPay.Core.DTOs.PaymentRequest;

public class CreatePaymentRequestDto
{
    [Required]
    [EmailAddress]
    public string RequesteeEmail { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Currency { get; set; } = "MKD";
}