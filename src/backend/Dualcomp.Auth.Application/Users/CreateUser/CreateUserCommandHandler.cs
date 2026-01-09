using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Dualcomp.Auth.Application.Services;

namespace Dualcomp.Auth.Application.Users.CreateUser
{
    public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailValidationRepository _emailValidationRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly ICompanySettingsService _companySettingsService;
        private readonly IConfiguration _configuration;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            IEmailValidationRepository emailValidationRepository,
            IPasswordHasher passwordHasher,
            IPasswordGenerator passwordGenerator,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IEmailTemplateService emailTemplateService,
            ICompanySettingsService companySettingsService,
            IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _emailValidationRepository = emailValidationRepository ?? throw new ArgumentNullException(nameof(emailValidationRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _emailTemplateService = emailTemplateService ?? throw new ArgumentNullException(nameof(emailTemplateService));
            _companySettingsService = companySettingsService ?? throw new ArgumentNullException(nameof(companySettingsService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<CreateUserResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Validaciones
            if (string.IsNullOrWhiteSpace(request.FirstName)) throw new ArgumentException("FirstName is required", nameof(request.FirstName));
            if (string.IsNullOrWhiteSpace(request.LastName)) throw new ArgumentException("LastName is required", nameof(request.LastName));
            if (string.IsNullOrWhiteSpace(request.Email)) throw new ArgumentException("Email is required", nameof(request.Email));

            var email = Email.Create(request.Email);

            // Verificar que el email no esté en uso
            var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Generar contraseña temporal
            var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();
            var hashedPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

            // Crear usuario
            var user = User.Create(
                request.FirstName,
                request.LastName,
                email,
                hashedPassword,
                request.CompanyId,
                request.IsCompanyAdmin);

            // Establecer contraseña temporal
            user.SetTemporaryPassword(temporaryPassword);

            // Guardar usuario
            await _userRepository.AddAsync(user, cancellationToken);

            // Generar token de validación de email
            var token = EmailValidationToken.GenerateWithTimestamp();
            var emailValidation = Domain.Users.EmailValidation.CreateWithDefaultExpiration(user.Id, token.Value);
            await _emailValidationRepository.AddAsync(emailValidation, cancellationToken);

            // Guardar cambios
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Enviar email de bienvenida
            await SendWelcomeEmailAsync(user, token.Value, cancellationToken);

            return new CreateUserResult(
                user.Id,
                user.Email.Value,
                user.GetFullName(),
                temporaryPassword,
                user.IsCompanyAdmin
            );
        }

        private async Task SendWelcomeEmailAsync(User user, string validationToken, CancellationToken cancellationToken)
        {
            try
            {
                // Obtener configuración SMTP de la empresa
                var companySettings = await _companySettingsService.GetOrCreateDefaultSmtpSettingsAsync(user.CompanyId, null, cancellationToken);
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

                // Crear y enviar email de bienvenida
                var emailMessage = _emailTemplateService.CreateWelcomeEmailTemplate(
                    user.Email.Value,
                    user.GetFullName(),
                    "Tu Empresa", // TODO: Obtener nombre de la empresa
                    validationToken,
                    baseUrl);

                await _emailService.SendEmailAsync(emailMessage, smtpConfig, cancellationToken);
            }
            catch (Exception)
            {
                // Log error but don't fail the user creation process
                // TODO: Implement proper logging
            }
        }
    }
}
