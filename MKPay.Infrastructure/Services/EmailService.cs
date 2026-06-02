// using MKPay.Core.Interfaces.Services;
// using Microsoft.Extensions.Configuration;
// using System.Net;
// using System.Net.Mail;
//
// namespace MKPay.Infrastructure.Services;
//
// public class EmailService : IEmailService
// {
//     private readonly IConfiguration _configuration;
//     private readonly bool _isEnabled;
//
//     public EmailService(IConfiguration configuration)
//     {
//         _configuration = configuration;
//         // Check if email is enabled (optional)
//         _isEnabled = _configuration.GetValue<bool>("EmailSettings:Enabled", false);
//     }
//
//     public async Task SendWelcomeEmailAsync(string toEmail, string userName)
//     {
//         if (!_isEnabled)
//         {
//             // Just log instead of sending (for demo)
//             Console.WriteLine($"[DEMO] Welcome email to {toEmail} for {userName}");
//             return;
//         }
//
//         var subject = "Welcome to MKPay!";
//         var body = $@"
//             <h2>Welcome to MKPay, {userName}!</h2>
//             <p>Your account has been created successfully.</p>
//             <p>You've received <strong>1000 MKD</strong> as a starting bonus!</p>
//             <p>Start sending and receiving money instantly.</p>
//             <br>
//             <p>Best regards,<br>MKPay Team</p>
//         ";
//
//         await SendEmailAsync(toEmail, subject, body);
//     }
//
//     public async Task SendTransactionNotificationAsync(string toEmail, string userName, decimal amount, string type)
//     {
//         if (!_isEnabled)
//         {
//             // Just log instead of sending (for demo)
//             Console.WriteLine($"[DEMO] Transaction notification to {toEmail}: {type} {amount} MKD");
//             return;
//         }
//
//         var subject = $"MKPay Transaction: {type}";
//         var body = $@"
//             <h2>Transaction Notification</h2>
//             <p>Hello {userName},</p>
//             <p>A transaction has been processed on your account:</p>
//             <ul>
//                 <li><strong>Type:</strong> {type}</li>
//                 <li><strong>Amount:</strong> {amount:F2} MKD</li>
//                 <li><strong>Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm}</li>
//             </ul>
//             <p>Your updated balance is available in your dashboard.</p>
//             <br>
//             <p>Best regards,<br>MKPay Team</p>
//         ";
//
//         await SendEmailAsync(toEmail, subject, body);
//     }
//
//     private async Task SendEmailAsync(string toEmail, string subject, string body)
//     {
//         try
//         {
//             // For demo: just log the email
//             Console.WriteLine($"Email sent to {toEmail}");
//             Console.WriteLine($"   Subject: {subject}");
//             Console.WriteLine($"   Body preview: {body.Substring(0, Math.Min(100, body.Length))}...");
//             
//             await Task.CompletedTask;
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Email failed: {ex.Message}");
//             // Don't throw - we don't want to break registration if email fails
//         }
//     }
// }

using MKPay.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace MKPay.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly bool _isEnabled;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
        _isEnabled = _configuration.GetValue<bool>("EmailSettings:Enabled", false);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "Welcome to MKPay!";
        var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #3d95ce 0%, #2d7eb8 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                    .button {{ background: #3d95ce; color: white; padding: 12px 30px; text-decoration: none; border-radius: 25px; display: inline-block; margin: 20px 0; }}
                    .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Welcome to MKPay!</h1>
                    </div>
                    <div class='content'>
                        <h2>Hello {userName}! </h2>
                        <p>Your account has been created successfully.</p>
                        <p><strong> You've received 1000 MKD as a starting bonus!</strong></p>
                        <p>You can now:</p>
                        <ul>
                            <li>Send money to friends and family</li>
                            <li>Request payments from others</li>
                            <li>Track your transaction history</li>
                        </ul>
                        <p style='text-align: center;'>
                            <a href='http://localhost:3000/login' class='button'>Start Using MKPay</a>
                        </p>
                        <div class='footer'>
                            <p>Best regards,<br>The MKPay Team</p>
                            <p>This is an automated message. Please do not reply to this email.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendTransactionNotificationAsync(string toEmail, string userName, decimal amount, string type)
    {
        var subject = $"MKPay Transaction: {type} 💰";
        var emoji = type == "Credit" ? "✅" : "📤";
        var action = type == "Credit" ? "received" : "sent";
        
        var body = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background: linear-gradient(135deg, #3d95ce 0%, #2d7eb8 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
                    .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
                    .transaction-box {{ background: white; padding: 20px; border-left: 4px solid #3d95ce; margin: 20px 0; }}
                    .amount {{ font-size: 32px; font-weight: bold; color: #3d95ce; }}
                    .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>{emoji} Transaction Notification</h1>
                    </div>
                    <div class='content'>
                        <h2>Hello {userName},</h2>
                        <p>A transaction has been processed on your account:</p>
                        
                        <div class='transaction-box'>
                            <p><strong>Type:</strong> {type}</p>
                            <p><strong>Amount:</strong></p>
                            <p class='amount'>{amount:F2} MKD</p>
                            <p><strong>Date:</strong> {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC</p>
                        </div>
                        
                        <p>You have {action} <strong>{amount:F2} MKD</strong>.</p>
                        <p>Your updated balance is now available in your dashboard.</p>
                        
                        <div class='footer'>
                            <p>Best regards,<br>The MKPay Team</p>
                            <p>If you didn't authorize this transaction, please contact us immediately.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        if (!_isEnabled)
        {
            Console.WriteLine($"[DEMO MODE] Email to {toEmail}");
            Console.WriteLine($"   Subject: {subject}");
            return;
        }

        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = _configuration.GetValue<int>("EmailSettings:SmtpPort");
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var password = _configuration["EmailSettings:Password"];

            using var smtpClient = new SmtpClient(smtpServer, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, password)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
            
            Console.WriteLine($"Real email sent successfully to {toEmail}");
            Console.WriteLine($"   Subject: {subject}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email to {toEmail}");
            Console.WriteLine($"   Error: {ex.Message}");
            
            // Don't throw - we don't want to break registration if email fails
        }
    }
}