using UMS.Models;

namespace UMS.Services
{
    public interface IEmailService
    {
        Task SendEmail(MailRequestModel model);
    }
}
