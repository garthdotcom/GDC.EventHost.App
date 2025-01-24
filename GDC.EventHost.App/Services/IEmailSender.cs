using GDC.EventHost.App.Models;

namespace GDC.EventHost.App.Services
{
    public interface IEmailSender
    {
        AuthMessageSenderOptions Options { get; }
        Task Execute(string apiKey, string subject, string message, string email);
        Task SendEmailAsync(string email, string subject, string message);
    }
}