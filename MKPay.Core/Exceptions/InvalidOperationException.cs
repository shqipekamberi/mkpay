namespace MKPay.Core.Exceptions;

public class InvalidTransactionException : MKPayException
{
    public InvalidTransactionException(string message) : base(message)
    {
    }
}