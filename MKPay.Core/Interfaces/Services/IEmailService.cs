namespace MKPay.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendTransactionNotificationAsync(string toEmail, string senderName, decimal amount, string currency);
    Task SendPaymentRequestNotificationAsync(string toEmail, string requesterName, decimal amount, string currency);
    Task SendWelcomeEmailAsync(string toEmail, string firstName);
}