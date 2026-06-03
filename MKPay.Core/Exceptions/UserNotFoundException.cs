namespace MKPay.Core.Exceptions;

public class UserNotFoundException : MKPayException
{
    public UserNotFoundException(string identifier) 
        : base($"User not found: {identifier}")
    {
    }
}