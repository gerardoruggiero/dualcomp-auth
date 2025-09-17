using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.Application.EmailValidation.SendValidationEmail
{
    public class SendValidationEmailCommandHandler : ICommandHandler<SendValidationEmailCommand, SendValidationEmailResult>
    {
        private readonly IEmailValidationRepository _emailValidationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly IConfiguration _configuration;

        public SendValidationEmailCommandHandler(
            IEmailValidationRepository emailValidationRepository,
            IUserRepository userRepository,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            SmtpConfiguration smtpConfiguration,
            IConfiguration configuration)
        {
            _emailValidationRepository = emailValidationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _smtpConfiguration = smtpConfiguration;
            _configuration = configuration;
        }

        public async Task<SendValidationEmailResult> Handle(SendValidationEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el usuario
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                
                if (user == null)
                {
                    return SendValidationEmailResult.Failure("Usuario no encontrado");
                }

                // Verificar si el email ya fue validado
                if (user.IsEmailValidated)
                {
                    return SendValidationEmailResult.Failure("El email ya ha sido validado");
                }

                // Verificar si ya existe un token activo para este usuario
                var hasActiveToken = await _emailValidationRepository.HasActiveTokenAsync(request.UserId, cancellationToken);
                
                if (hasActiveToken)
                {
                    return SendValidationEmailResult.Failure("Ya existe un token de validación activo para este usuario");
                }

                // Generar nuevo token de validación
                var token = EmailValidationToken.GenerateWithTimestamp();
                var emailValidation = Domain.Users.EmailValidation.CreateWithDefaultExpiration(request.UserId, token.Value);

                // Guardar el token en la base de datos
                await _emailValidationRepository.AddAsync(emailValidation, cancellationToken);

                // Crear y enviar email de validación
                var baseUrl = _configuration["ApplicationSettings:BaseUrl"] ?? "https://localhost:5001";
                var emailMessage = _emailTemplateService.CreateEmailValidationTemplate(
                    user.Email.Value,
                    user.GetFullName(),
                    token.Value,
                    baseUrl);

                var emailResult = await _emailService.SendEmailAsync(emailMessage, _smtpConfiguration, cancellationToken);

                if (!emailResult.IsSuccess)
                {
                    // Si falla el envío, eliminar el token creado
                    await _emailValidationRepository.DeleteAsync(emailValidation, cancellationToken);
                    return SendValidationEmailResult.Failure($"Error al enviar email: {emailResult.ErrorMessage}");
                }
                
                return SendValidationEmailResult.Success(token.Value, emailValidation.ExpiresAt);
            }
            catch (Exception ex)
            {
                return SendValidationEmailResult.Failure($"Error al generar el token de validación: {ex.Message}");
            }
        }
    }
}
