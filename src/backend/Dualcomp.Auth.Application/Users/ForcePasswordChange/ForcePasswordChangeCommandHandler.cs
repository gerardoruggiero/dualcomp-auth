using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.Application.Users.ForcePasswordChange
{
    public class ForcePasswordChangeCommandHandler : ICommandHandler<ForcePasswordChangeCommand, ForcePasswordChangeResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly JwtSettings _jwtSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly IConfiguration _configuration;

        public ForcePasswordChangeCommandHandler(
            IUserRepository userRepository,
            IUserSessionRepository userSessionRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService,
            JwtSettings jwtSettings,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            SmtpConfiguration smtpConfiguration,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userSessionRepository = userSessionRepository ?? throw new ArgumentNullException(nameof(userSessionRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
            _jwtSettings = jwtSettings ?? throw new ArgumentNullException(nameof(jwtSettings));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _smtpConfiguration = smtpConfiguration ?? throw new ArgumentNullException(nameof(smtpConfiguration));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<ForcePasswordChangeResult> Handle(ForcePasswordChangeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar el usuario
                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
                if (user == null)
                {
                    return ForcePasswordChangeResult.Failure("Usuario no encontrado");
                }

                // Verificar que el usuario requiere cambio de contraseña
                if (!user.RequiresPasswordChange())
                {
                    return ForcePasswordChangeResult.Failure("Este usuario no requiere cambio de contraseña");
                }

                // Verificar la contraseña temporal
                if (!user.HasValidTemporaryPassword(request.TemporaryPassword))
                {
                    return ForcePasswordChangeResult.Failure("Contraseña temporal inválida");
                }

                // Validar la nueva contraseña
                var newPassword = Password.Create(request.NewPassword);
                var hashedNewPassword = HashedPassword.Create(_passwordHasher.HashPassword(newPassword.Value));

                // Actualizar la contraseña del usuario
                user.UpdatePassword(hashedNewPassword);
                user.ClearTemporaryPassword();

                // Invalidar todas las sesiones previas del usuario
                await _userSessionRepository.DeactivateAllUserSessionsAsync(user.Id, cancellationToken);

                // Generar nueva sesión
                var sessionId = Guid.NewGuid();
                var refreshToken = _jwtTokenService.GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

                // Crear nueva sesión
                var session = UserSession.Create(
                    user.Id,
                    string.Empty, // accessToken se actualizará después
                    refreshToken,
                    expiresAt,
                    "ForcePasswordChange", // UserAgent especial
                    "127.0.0.1"); // IP especial

                // Generar access token
                var accessToken = _jwtTokenService.GenerateAccessToken(
                    user.Id,
                    user.Email.Value,
                    user.CompanyId,
                    sessionId,
                    user.IsCompanyAdmin);

                // Actualizar la sesión con el access token
                session.UpdateTokens(accessToken, refreshToken, expiresAt);

                // Guardar cambios
                await _userSessionRepository.AddAsync(session, cancellationToken);
                await _userRepository.UpdateAsync(user, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Enviar email de confirmación (opcional)
                try
                {
                    var baseUrl = _configuration["ApplicationSettings:BaseUrl"] ?? "https://localhost:5001";
                    var emailMessage = CreatePasswordChangeConfirmationEmail(user, baseUrl);
                    await _emailService.SendEmailAsync(emailMessage, _smtpConfiguration, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the operation
                    // TODO: Add logging
                }

                return ForcePasswordChangeResult.Success(accessToken, refreshToken, expiresAt);
            }
            catch (Exception ex)
            {
                return ForcePasswordChangeResult.Failure($"Error al cambiar la contraseña: {ex.Message}");
            }
        }

        private EmailMessage CreatePasswordChangeConfirmationEmail(User user, string baseUrl)
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Contraseña Cambiada - DualComp CRM</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #27ae60; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f8f9fa; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .success {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>DualComp CRM</h1>
        </div>
        <div class='content'>
            <h2>¡Contraseña Actualizada!</h2>
            <p>Hola {user.GetFullName()},</p>
            <div class='success'>
                <p><strong>✅ ¡Perfecto!</strong></p>
                <p>Tu contraseña ha sido actualizada exitosamente.</p>
            </div>
            <p>Ahora puedes:</p>
            <ul>
                <li>Iniciar sesión normalmente con tu nueva contraseña</li>
                <li>Acceder a todas las funcionalidades del CRM</li>
                <li>Cambiar tu contraseña nuevamente cuando lo desees</li>
            </ul>
            <p>Si no realizaste este cambio, contacta inmediatamente al administrador del sistema.</p>
        </div>
        <div class='footer'>
            <p>Este email fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
            <p>&copy; 2025 DualComp CRM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = user.Email.Value,
                ToName = user.GetFullName(),
                Subject = "Contraseña Actualizada - DualComp CRM",
                Body = htmlBody,
                IsHtml = true
            };
        }
    }
}

