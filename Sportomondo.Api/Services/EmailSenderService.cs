using Sportomondo.Api.Helpers;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace Sportomondo.Api.Services
{
    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtp = "poczta.interia.pl";
        private readonly int _port = 587;

        public EmailSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailsAsync(IEnumerable<EmailTemplate> emails)
        {
            try
            {
                using (var client = new SmtpClient(_smtp, _port))
                {
                    var credentialsPath = _configuration.GetValue<string>("EmailSenderAccountCredentialsPath"); //first line - login, second line - password
                    var credentials = File.ReadAllLines(credentialsPath);
                    var login = credentials[0];
                    var password = credentials[1];

                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(login, password);

                    foreach (var email in emails)
                    {
                        var message = new MailMessage(login, email.To, email.Subject, email.Body);

                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
