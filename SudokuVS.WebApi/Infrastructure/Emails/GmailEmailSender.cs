using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace SudokuVS.WebApi.Infrastructure.Emails;

class GmailEmailSender : IEmailSender
{
    readonly IOptionsMonitor<GmailAccountConfiguration> _options;
    readonly ILogger<GmailEmailSender> _logger;

    public GmailEmailSender(IOptionsMonitor<GmailAccountConfiguration> options, ILogger<GmailEmailSender> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        string senderEmail = _options.CurrentValue.Email;
        NetworkCredential credentials = new(senderEmail, _options.CurrentValue.Password);

        using MailMessage message = new(senderEmail, email, subject, htmlMessage);
        message.IsBodyHtml = true;
        using SmtpClient client = CreateClient(credentials);

        _logger.LogDebug("Sending email from {sender} to {recipient} with subject {subject}...", senderEmail, email, subject);

        await client.SendMailAsync(message);
    }

    static SmtpClient CreateClient(NetworkCredential networkCredential)
    {
        SmtpClient result = new();
        result.Host = "smtp.gmail.com";
        result.EnableSsl = true;
        result.UseDefaultCredentials = true;
        result.Credentials = networkCredential;
        result.Port = 465;
        return result;
    }
}

class GmailAccountConfiguration
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}
