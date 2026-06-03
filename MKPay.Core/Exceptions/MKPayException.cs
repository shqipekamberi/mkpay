namespace MKPay.Core.Exceptions;

public class MKPayException : Exception
{
    public MKPayException(string message) : base(message)
    {
    }

    public MKPayException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}