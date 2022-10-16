using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AlkemyChallenge.Services.SendGrid
{
    public class Sendgrid
    {
        public static async Task sendEmail(string email, string username)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var sender = Environment.GetEnvironmentVariable("EMAIL_SENDER");
            var senderUsername = Environment.GetEnvironmentVariable("SENDER_USERNAME");
            var client = new SendGridClient(apiKey);
            var to = new EmailAddress(email, username);
            var subject = "Welcome to the Disney API";
            var from = new EmailAddress(sender, senderUsername);
            var plainTextContent = "API part of the AlkemyChallenge";
            var htmlContent = "<a href= 'https://github.com/pablodclavijo'><a>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
