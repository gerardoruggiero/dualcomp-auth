using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;
using Dualcomp.Auth.Application.Services;

namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	public class RegisterCompanyCommandHandler : ICommandHandler<RegisterCompanyCommand, RegisterCompanyResult>
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly ICompanyContactService _contactService;
		private readonly IUserRepository _userRepository;
		private readonly IEmailValidationRepository _emailValidationRepository;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IPasswordGenerator _passwordGenerator;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IEmailService _emailService;
		private readonly IEmailTemplateService _emailTemplateService;
        private readonly ICompanySettingsService _companySettingsService;
		private readonly IConfiguration _configuration;

		public RegisterCompanyCommandHandler(
			ICompanyRepository companyRepository,
			ICompanyContactService contactService,
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
			_companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
			_contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
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

		public async Task<RegisterCompanyResult> Handle(RegisterCompanyCommand command, CancellationToken cancellationToken)
		{
			// Validaciones básicas
			if (string.IsNullOrWhiteSpace(command.Name)) throw new ArgumentException("Name is required", nameof(command.Name));
			if (string.IsNullOrWhiteSpace(command.TaxId)) throw new ArgumentException("TaxId is required", nameof(command.TaxId));
			if (!command.Employees?.Any() == true) throw new ArgumentException("At least one employee is required", nameof(command.Employees));

			// Validar contactos requeridos usando el servicio
			_contactService.ValidateRequiredContactsForRegistration(command.Addresses, command.Emails, command.Phones, command.SocialMedias);

			var taxId = TaxId.Create(command.TaxId);
			// uniqueness check
			var exists = await _companyRepository.ExistsByTaxIdAsync(taxId.Value, cancellationToken);
			if (exists) throw new InvalidOperationException("A company with the same TaxId already exists");

			// Crear la empresa
			var company = Company.Create(command.Name, taxId);

			// Procesar todos los contactos usando el servicio
			var contactTypeNames = await _contactService.ProcessAllContactsForRegistrationAsync(
				company, 
				command.Addresses, 
				command.Emails, 
				command.Phones, 
				command.SocialMedias, 
				cancellationToken);

			// Procesar empleados usando el servicio unificado
			await _contactService.ProcessEmployeesForRegistrationAsync(company, command.Employees, cancellationToken);

			// Validar que la empresa esté completa para registro
			if (!company.IsValidForRegistration())
			{
				throw new InvalidOperationException("Company does not meet registration requirements");
			}

			await _companyRepository.AddAsync(company, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

			// Enviar emails de notificación después de guardar exitosamente
			await SendCompanyRegistrationEmailsAsync(company, command.Employees, cancellationToken);

			return new RegisterCompanyResult(
				company.Id,
				company.Name,
				company.TaxId.Value,
				_contactService.BuildAddressResults(company, contactTypeNames.AddressTypeNames),
				_contactService.BuildEmailResults(company, contactTypeNames.EmailTypeNames),
				_contactService.BuildPhoneResults(company, contactTypeNames.PhoneTypeNames),
				_contactService.BuildSocialMediaResults(company, contactTypeNames.SocialMediaTypeNames),
				_contactService.BuildEmployeeResults(company)
			);
		}

		private async Task SendCompanyRegistrationEmailsAsync(Company company, List<RegisterCompanyEmployeeDto> employees, CancellationToken cancellationToken)
		{
			try
			{
				// Obtener configuración SMTP de la empresa o usar la por defecto
				var companySettings = await _companySettingsService.GetOrCreateDefaultSmtpSettingsAsync(company.Id, null, cancellationToken);
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

				// Enviar email a cada empleado con sus credenciales
				foreach (var employee in employees)
				{
					// Buscar el usuario creado para este empleado
					var user = await _userRepository.GetByEmailAsync(Email.Create(employee.Email), cancellationToken);
					if (user != null)
					{
						// Generar token de validación de email
						var token = Domain.Users.ValueObjects.EmailValidationToken.GenerateWithTimestamp();
						var emailValidation = Domain.Users.EmailValidation.CreateWithDefaultExpiration(user.Id, token.Value);
						
						// Guardar el token en la base de datos
						await _emailValidationRepository.AddAsync(emailValidation, cancellationToken);

						// Crear y enviar email de bienvenida con credenciales
						var emailMessage = _emailTemplateService.CreateWelcomeEmailTemplate(
							employee.Email,
							employee.FullName,
							company.Name,
							token.Value,
							baseUrl);

						await _emailService.SendEmailAsync(emailMessage, smtpConfig, cancellationToken);
					}
				}
			}
			catch (Exception)
			{
				// Log error but don't fail the registration process
				// TODO: Implement proper logging
			}
		}
	}
}
