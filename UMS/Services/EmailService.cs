using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using System.Security.Authentication;
using UMS.Models;
using MailKit.Security;

namespace UMS.Services;

public class EmailService: IEmailService
{
    private readonly EmailSettingsModel emailSettings;
    public EmailService(IOptions<EmailSettingsModel> emailSettings)
    {
        this.emailSettings = emailSettings.Value;
    }
    public async Task SendEmail(MailRequestModel request)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(emailSettings.Email);
        email.To.Add(MailboxAddress.Parse(request.Email));
        email.Subject = request.Subject;
        var builder = new BodyBuilder
        {
            HtmlBody = request.Body
        };
        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(emailSettings.Email, emailSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

    }
}

