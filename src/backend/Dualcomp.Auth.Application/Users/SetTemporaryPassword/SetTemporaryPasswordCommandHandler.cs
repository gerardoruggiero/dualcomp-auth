using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.Application.Users.SetTemporaryPassword
{
    public class SetTemporaryPasswordCommandHandler : ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly IConfiguration _configuration;

        public SetTemporaryPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            SmtpConfiguration smtpConfiguration,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _smtpConfiguration = smtpConfiguration ?? throw new ArgumentNullException(nameof(smtpConfiguration));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<SetTemporaryPasswordResult> Handle(SetTemporaryPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Verificar que el admin existe y es administrador
                var adminUser = await _userRepository.GetByIdAsync(request.AdminUserId, cancellationToken);
                if (adminUser == null || !adminUser.IsCompanyAdmin)
                {
                    return SetTemporaryPasswordResult.Failure("No tienes permisos para realizar esta acción");
                }

                // Buscar el usuario objetivo
                var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
                if (targetUser == null)
                {
                    return SetTemporaryPasswordResult.Failure("Usuario no encontrado");
                }

                // Verificar que ambos usuarios pertenecen a la misma empresa
                if (adminUser.CompanyId != targetUser.CompanyId)
                {
                    return SetTemporaryPasswordResult.Failure("No puedes establecer contraseñas para usuarios de otras empresas");
                }

                // Generar contraseña temporal si no se proporciona
                var temporaryPassword = string.IsNullOrWhiteSpace(request.TemporaryPassword) 
                    ? GenerateTemporaryPassword() 
                    : request.TemporaryPassword;

                // Hashear la contraseña temporal
                var hashedTemporaryPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

                // Establecer la contraseña temporal
                targetUser.UpdatePassword(hashedTemporaryPassword);
                targetUser.SetTemporaryPassword(temporaryPassword);

                // Guardar cambios
                await _userRepository.UpdateAsync(targetUser, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Enviar email con la contraseña temporal
                try
                {
                    var baseUrl = _configuration["ApplicationSettings:BaseUrl"] ?? "https://localhost:5001";
                    var emailMessage = _emailTemplateService.CreateUserCreatedTemplate(
                        targetUser.Email.Value,
                        targetUser.GetFullName(),
                        temporaryPassword,
                        baseUrl);

                    await _emailService.SendEmailAsync(emailMessage, _smtpConfiguration, cancellationToken);
                }
                catch (Exception ex)
                {
                    // Log error but don't fail the operation
                    // TODO: Add logging
                }

                return SetTemporaryPasswordResult.Success(targetUser.Email.Value, temporaryPassword);
            }
            catch (Exception ex)
            {
                return SetTemporaryPasswordResult.Failure($"Error al establecer contraseña temporal: {ex.Message}");
            }
        }

        private string GenerateTemporaryPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

