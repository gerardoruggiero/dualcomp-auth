using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	public class RegisterCompanyCommandHandler : ICommandHandler<RegisterCompanyCommand, RegisterCompanyResult>
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly CompanyContactService _contactService;
		private readonly IUserRepository _userRepository;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IPasswordGenerator _passwordGenerator;
		private readonly IUnitOfWork _unitOfWork;

		public RegisterCompanyCommandHandler(
			ICompanyRepository companyRepository,
			CompanyContactService contactService,
			IUserRepository userRepository,
			IPasswordHasher passwordHasher,
			IPasswordGenerator passwordGenerator,
			IUnitOfWork unitOfWork)
		{
			_companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
			_contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
			_userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
			_passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
			_passwordGenerator = passwordGenerator ?? throw new ArgumentNullException(nameof(passwordGenerator));
			_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		}

		public async Task<RegisterCompanyResult> Handle(RegisterCompanyCommand command, CancellationToken cancellationToken)
		{
			// Validaciones básicas
			if (string.IsNullOrWhiteSpace(command.Name)) throw new ArgumentException("Name is required", nameof(command.Name));
			if (string.IsNullOrWhiteSpace(command.TaxId)) throw new ArgumentException("TaxId is required", nameof(command.TaxId));
			if (!command.Employees?.Any() == true) throw new ArgumentException("At least one employee is required", nameof(command.Employees));

			// Validar contactos requeridos usando el servicio
			_contactService.ValidateRequiredContacts(command.Addresses, command.Emails, command.Phones, command.SocialMedias);

			var taxId = TaxId.Create(command.TaxId);
			// uniqueness check
			var exists = await _companyRepository.ExistsByTaxIdAsync(taxId.Value, cancellationToken);
			if (exists) throw new InvalidOperationException("A company with the same TaxId already exists");

			// Crear la empresa
			var company = Company.Create(command.Name, taxId);

			// Procesar todos los contactos usando el servicio
			var contactTypeNames = await _contactService.ProcessAllContactsAsync(
				company, 
				command.Addresses, 
				command.Emails, 
				command.Phones, 
				command.SocialMedias, 
				cancellationToken);

			// Agregar empleados y crear usuarios automáticamente
			foreach (var employeeDto in command.Employees)
			{
				// Crear usuario del sistema automáticamente para cada empleado
				var user = await _contactService.CreateUserForEmployee(employeeDto.FullName, employeeDto.Email, company.Id, cancellationToken);

                var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate, user.Id);
                company.AddEmployee(employee);
            }

			// Validar que la empresa esté completa para registro
			if (!company.IsValidForRegistration())
			{
				throw new InvalidOperationException("Company does not meet registration requirements");
			}

			await _companyRepository.AddAsync(company, cancellationToken);
			await _unitOfWork.SaveChangesAsync(cancellationToken);

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
	}
}
