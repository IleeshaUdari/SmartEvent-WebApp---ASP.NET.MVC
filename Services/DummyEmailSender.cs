using Microsoft.AspNetCore.Identity.UI.Services;

namespace SmartEventWeb.Services
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For assignment demo: no real email sending
            return Task.CompletedTask;
        }
    }
}
