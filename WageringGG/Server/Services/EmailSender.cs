using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace WageringGG.Server.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string apiKey;

        public EmailSender(IConfiguration config)
        {
            apiKey = config["SendGridKey"];
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage();
            msg.SetFrom(new EmailAddress("ismellike@live.com", "Wagering.GG (ismellike)"));
            msg.AddTo(new EmailAddress(email));
            msg.SetSubject(subject);
            msg.AddContent(MimeType.Html, htmlMessage);
            return client.SendEmailAsync(msg);
        }
    }
}
