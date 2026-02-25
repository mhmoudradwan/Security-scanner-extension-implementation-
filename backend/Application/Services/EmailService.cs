using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Application.Services;

public class EmailService : Interfaces.IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPasswordResetEmailAsync(string email, string resetToken)
    {
        var smtpHost = _configuration["Smtp:Host"] ?? "smtp.gmail.com";
        var smtpPort = int.Parse(_configuration["Smtp:Port"] ?? "587");
        var smtpUser = _configuration["Smtp:Username"] ?? "";
        var smtpPass = _configuration["Smtp:Password"] ?? "";
        var fromEmail = _configuration["Smtp:FromEmail"] ?? smtpUser;
        var fromName = _configuration["Smtp:FromName"] ?? "Baseera Security Scanner";
        var appUrl = _configuration["AppUrl"] ?? "http://localhost:5173";

        var resetUrl = $"{appUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}";

        var message = new MailMessage
        {
            From = new MailAddress(fromEmail, fromName),
            Subject = "Password Reset Request - Baseera",
            IsBodyHtml = true,
            Body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #333;'>Password Reset Request</h2>
                    <p>You requested a password reset for your Baseera Security Scanner account.</p>
                    <p>Click the button below to reset your password. This link expires in <strong>15 minutes</strong>.</p>
                    <a href='{resetUrl}' style='display: inline-block; padding: 12px 24px; background-color: #4f46e5; color: white; text-decoration: none; border-radius: 6px; margin: 16px 0;'>
                        Reset Password
                    </a>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <p style='color: #666; font-size: 12px;'>This link will expire in 15 minutes.</p>
                </div>"
        };

        message.To.Add(email);

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUser, smtpPass)
        };

        await client.SendMailAsync(message);
    }
}
