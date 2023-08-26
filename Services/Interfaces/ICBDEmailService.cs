using Microsoft.AspNetCore.Identity.UI.Services;

namespace CBD.Services.Interfaces
{
    public interface ICBDEmailService : IEmailSender
    {
        Task SendContactEmailAsync(string emailFrom, string name, string subject, string htmlMessage);

    }
}
