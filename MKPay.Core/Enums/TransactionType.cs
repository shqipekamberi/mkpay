namespace MKPay.Core.Enums;

public enum TransactionType
{
    Transfer = 0,        // Direct P2P transfer
    PaymentRequest = 1,  // Requesting money
    BillSplit = 2,       // Split bill payment
    Refund = 3          // Refunded transaction
}