namespace MKPay.Core.Exceptions;

public class UnauthorizedOperationException : MKPayException
{
    public UnauthorizedOperationException(string message = "Unauthorized access to resource") 
        : base(message)
    {
    }
}