namespace MKPay.Infrastructure.Utils;

public static class TransactionReferenceGenerator
{
    /// <summary>
    /// Generates a unique transaction reference number
    /// Format: TXN-YYYYMMDD-XXXXXX
    /// Example: TXN-20260131-A3F9K2
    /// </summary>
    public static string Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = GenerateRandomString(6);
        
        return $"TXN-{datePart}-{randomPart}";
    }
    
    private static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Removed confusing chars
        var random = new Random();
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
        {
            result[i] = chars[random.Next(chars.Length)];
        }
        
        return new string(result);
    }
}