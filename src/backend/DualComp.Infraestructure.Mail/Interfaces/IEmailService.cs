using DualComp.Infraestructure.Mail.Models;

namespace DualComp.Infraestructure.Mail.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default);
        Task<EmailResult> SendEmailAsync(EmailMessage message, SmtpConfiguration smtpConfig, CancellationToken cancellationToken = default);
        Task<bool> TestConnectionAsync(SmtpConfiguration smtpConfig, CancellationToken cancellationToken = default);
    }
}

