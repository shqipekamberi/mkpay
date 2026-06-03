namespace MKPay.Core.DTOs.Transaction;

public class TransactionResponseDto
{
    public Guid Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    
    // Sender info
    public string SenderName { get; set; } = string.Empty;
    public string SenderEmail { get; set; } = string.Empty;
    
    // Receiver info
    public string ReceiverName { get; set; } = string.Empty;
    public string ReceiverEmail { get; set; } = string.Empty;
}