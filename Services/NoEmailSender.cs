using Microsoft.AspNetCore.Identity.UI.Services;

namespace SmartEventWeb.Services
{
    public class NoEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // For assignments / local dev: do nothing (no real email sending)
            return Task.CompletedTask;
        }
    }
}
