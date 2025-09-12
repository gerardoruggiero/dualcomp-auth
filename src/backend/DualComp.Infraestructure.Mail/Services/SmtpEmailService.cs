using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Mail;

namespace DualComp.Infraestructure.Mail.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly SmtpConfiguration _defaultSmtpConfig;

        public SmtpEmailService(ILogger<SmtpEmailService> logger, SmtpConfiguration defaultSmtpConfig)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _defaultSmtpConfig = defaultSmtpConfig ?? throw new ArgumentNullException(nameof(defaultSmtpConfig));
        }

        public async Task<EmailResult> SendEmailAsync(EmailMessage message, CancellationToken cancellationToken = default)
        {
            return await SendEmailAsync(message, _defaultSmtpConfig, cancellationToken);
        }

        public async Task<EmailResult> SendEmailAsync(EmailMessage message, SmtpConfiguration smtpConfig, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (smtpConfig == null) throw new ArgumentNullException(nameof(smtpConfig));

            try
            {
                _logger.LogInformation("Iniciando envío de email a {To}", message.To);

                using var smtpClient = CreateSmtpClient(smtpConfig);
                using var mailMessage = CreateMailMessage(message, smtpConfig);

                await smtpClient.SendMailAsync(mailMessage);

                _logger.LogInformation("Email enviado exitosamente a {To}", message.To);
                return EmailResult.Success($"Email enviado exitosamente a {message.To}");
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Envío de email cancelado para {To}", message.To);
                return EmailResult.Failure("Envío de email cancelado");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {To}: {ErrorMessage}", message.To, ex.Message);
                return EmailResult.Failure($"Error al enviar email: {ex.Message}");
            }
        }

        public async Task<bool> TestConnectionAsync(SmtpConfiguration smtpConfig, CancellationToken cancellationToken = default)
        {
            if (smtpConfig == null) throw new ArgumentNullException(nameof(smtpConfig));

            try
            {
                _logger.LogInformation("Probando conexión SMTP a {Server}:{Port}", smtpConfig.Server, smtpConfig.Port);

                using var smtpClient = CreateSmtpClient(smtpConfig);
                
                // Crear un mensaje de prueba simple
                using var testMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig.FromEmail, smtpConfig.FromName),
                    Subject = "Test Connection",
                    Body = "This is a test message to verify SMTP connection.",
                    IsBodyHtml = false
                };
                testMessage.To.Add(smtpConfig.FromEmail); // Enviar a nosotros mismos

                await smtpClient.SendMailAsync(testMessage);

                _logger.LogInformation("Conexión SMTP exitosa a {Server}:{Port}", smtpConfig.Server, smtpConfig.Port);
                return true;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Prueba de conexión SMTP cancelada para {Server}:{Port}", smtpConfig.Server, smtpConfig.Port);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al probar conexión SMTP a {Server}:{Port}: {ErrorMessage}", smtpConfig.Server, smtpConfig.Port, ex.Message);
                return false;
            }
        }

        private SmtpClient CreateSmtpClient(SmtpConfiguration smtpConfig)
        {
            var smtpClient = new SmtpClient
            {
                Host = smtpConfig.Server,
                Port = smtpConfig.Port,
                EnableSsl = smtpConfig.UseSsl,
                Timeout = smtpConfig.Timeout,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            // Configurar credenciales si se proporcionan
            if (!string.IsNullOrEmpty(smtpConfig.Username) && !string.IsNullOrEmpty(smtpConfig.Password))
            {
                smtpClient.Credentials = new NetworkCredential(smtpConfig.Username, smtpConfig.Password);
            }

            return smtpClient;
        }

        private MailMessage CreateMailMessage(EmailMessage message, SmtpConfiguration smtpConfig)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(
                    !string.IsNullOrEmpty(message.From) ? message.From : smtpConfig.FromEmail,
                    !string.IsNullOrEmpty(message.FromName) ? message.FromName : smtpConfig.FromName
                ),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = message.IsHtml,
                Priority = MailPriority.Normal
            };

            // To
            mailMessage.To.Add(new MailAddress(message.To, message.ToName ?? message.To));

            // CC
            if (message.Cc != null && message.Cc.Any())
            {
                foreach (var cc in message.Cc)
                {
                    mailMessage.CC.Add(new MailAddress(cc));
                }
            }

            // BCC
            if (message.Bcc != null && message.Bcc.Any())
            {
                foreach (var bcc in message.Bcc)
                {
                    mailMessage.Bcc.Add(new MailAddress(bcc));
                }
            }

            // Attachments
            if (message.Attachments != null && message.Attachments.Any())
            {
                foreach (var attachment in message.Attachments)
                {
                    var stream = new MemoryStream(attachment.Content);
                    var mailAttachment = new Attachment(stream, attachment.FileName, attachment.ContentType);
                    mailMessage.Attachments.Add(mailAttachment);
                }
            }

            return mailMessage;
        }
    }
}