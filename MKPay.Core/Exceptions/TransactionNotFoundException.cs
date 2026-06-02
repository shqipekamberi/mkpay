namespace MKPay.Core.Exceptions;

public class TransactionNotFoundException : MKPayException
{
    public TransactionNotFoundException(Guid transactionId) 
        : base($"Transaction not found: {transactionId}")
    {
    }
}