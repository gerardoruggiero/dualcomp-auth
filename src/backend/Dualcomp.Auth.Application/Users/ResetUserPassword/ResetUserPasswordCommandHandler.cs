using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Dualcomp.Auth.Application.Services;

namespace Dualcomp.Auth.Application.Users.ResetUserPassword
{
    public class ResetUserPasswordCommandHandler : ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IConfiguration _configuration;

        public ResetUserPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IPasswordGenerator passwordGenerator,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ICompanySettingsService companySettingsService,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<ResetUserPasswordResult> Handle(ResetUserPasswordCommand request, CancellationToken cancellationToken)
        {
            // Verificar que el admin existe y es administrador
            var adminUser = await _userRepository.GetByIdAsync(request.AdminUserId, cancellationToken);
            if (adminUser == null)
            {
                return ResetUserPasswordResult.Failure("Administrador no encontrado");
            }

            if (!adminUser.IsCompanyAdmin)
            {
                return ResetUserPasswordResult.Failure("No tienes permisos para realizar esta acción");
            }

            // Buscar el usuario objetivo
            var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
            if (targetUser == null)
            {
                return ResetUserPasswordResult.Failure("Usuario no encontrado");
            }

            // Verificar que ambos usuarios pertenecen a la misma empresa
            if (adminUser.CompanyId != targetUser.CompanyId)
            {
                return ResetUserPasswordResult.Failure("No puedes reiniciar la contraseña de usuarios de otras empresas");
            }

            // Generar nueva contraseña temporal
            var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();
            var hashedPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

            // Actualizar contraseña del usuario
            targetUser.UpdatePassword(hashedPassword);
            targetUser.SetTemporaryPassword(temporaryPassword);
            targetUser.SetMustChangePassword(true);

            // Guardar cambios
            await _userRepository.UpdateAsync(targetUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Enviar email con la nueva contraseña
            await SendPasswordResetEmailAsync(targetUser, temporaryPassword, cancellationToken);

            return ResetUserPasswordResult.Success(
                targetUser.Id,
                targetUser.Email.Value,
                targetUser.GetFullName(),
                temporaryPassword,
                "Contraseña reiniciada exitosamente. Se ha enviado un email con la nueva contraseña temporal."
            );
        }

        private async Task SendPasswordResetEmailAsync(User user, string temporaryPassword, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener configuración SMTP de la empresa
                var companySettings = await _companySettingsService.GetOrCreateDefaultSmtpSettingsAsync(user.CompanyId ?? Guid.Empty, null, cancellationToken);
                var smtpConfig = new SmtpConfiguration
                {
                    Server = companySettings.SmtpServer,
                    Port = companySettings.SmtpPort,
                    Username = companySettings.SmtpUsername,
                    Password = companySettings.SmtpPassword,
                    UseSsl = companySettings.SmtpUseSsl,
                    FromEmail = companySettings.SmtpFromEmail,
                    FromName = companySettings.SmtpFromName,
                    Timeout = 30000 // Default timeout
                };

                var baseUrl = _configuration["ApplicationSettings:BaseUrl"] ?? "https://localhost:5001";

                // Crear y enviar email con nueva contraseña
                var emailMessage = _emailTemplateService.CreateUserCreatedTemplate(
                    user.Email.Value,
                    user.GetFullName(),
                    temporaryPassword,
                    baseUrl);

                await _emailService.SendEmailAsync(emailMessage, smtpConfig, cancellationToken);
            }
            catch (Exception)
            {
                // Log error but don't fail the password reset process
                // TODO: Implement proper logging
            }
        }
    }
}
