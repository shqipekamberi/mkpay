namespace MKPay.Infrastructure.Utils;

public static class AccountNumberGenerator
{
    private static readonly Random _random = new Random();
    
    /// <summary>
    /// Generates a unique 16-digit account number
    /// Format: 6220 XXXX XXXX XXXX (6220 is MKPay prefix)
    /// </summary>
    public static string Generate()
    {
        var prefix = "6220"; // MKPay identifier
        var remainingDigits = 12;
        var accountNumber = prefix;
        
        for (int i = 0; i < remainingDigits; i++)
        {
            accountNumber += _random.Next(0, 10).ToString();
        }
        
        return accountNumber;
    }
    
    /// <summary>
    /// Formats account number with spaces for display
    /// Example: 6220123456789012 -> 6220 1234 5678 9012
    /// </summary>
    public static string Format(string accountNumber)
    {
        if (string.IsNullOrEmpty(accountNumber) || accountNumber.Length != 16)
            return accountNumber;
            
        return $"{accountNumber.Substring(0, 4)} {accountNumber.Substring(4, 4)} {accountNumber.Substring(8, 4)} {accountNumber.Substring(12, 4)}";
    }
}