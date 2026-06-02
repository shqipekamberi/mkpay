public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string userName);
    Task SendTransactionNotificationAsync(string toEmail, string userName, decimal amount, string type);
}