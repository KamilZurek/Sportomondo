using Sportomondo.Api.Helpers;

namespace Sportomondo.Api.Services
{
    public interface IEmailSenderService
    {
        Task SendEmailsAsync(IEnumerable<EmailTemplate> emails);
    }
}
