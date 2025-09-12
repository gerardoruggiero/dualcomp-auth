using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.Application.Companies.RegisterCompany
{
	public class RegisterCompanyCommandHandler : ICommandHandler<RegisterCompanyCommand, RegisterCompanyResult>
	{
		private readonly ICompanyRepository _companyRepository;
		private readonly IAddressTypeRepository _addressTypeRepository;
		private readonly IEmailTypeRepository _emailTypeRepository;
		private readonly IPhoneTypeRepository _phoneTypeRepository;
		private readonly ISocialMediaTypeRepository _socialMediaTypeRepository;
		private readonly IUserRepository _userRepository;
		private readonly IPasswordHasher _passwordHasher;
		private readonly IPasswordGenerator _passwordGenerator;
		private readonly IUnitOfWork _unitOfWork;

		public RegisterCompanyCommandHandler(
			ICompanyRepository companyRepository,
			IAddressTypeRepository addressTypeRepository,
			IEmailTypeRepository emailTypeRepository,
			IPhoneTypeRepository phoneTypeRepository,
			ISocialMediaTypeRepository socialMediaTypeRepository,
			IUserRepository userRepository,
			IPasswordHasher passwordHasher,
			IPasswordGenerator passwordGenerator,
			IUnitOfWork unitOfWork)
		{
			_companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
			_addressTypeRepository = addressTypeRepository ?? throw new ArgumentNullException(nameof(addressTypeRepository));
			_emailTypeRepository = emailTypeRepository ?? throw new ArgumentNullException(nameof(emailTypeRepository));
			_phoneTypeRepository = phoneTypeRepository ?? throw new ArgumentNullException(nameof(phoneTypeRepository));
			_socialMediaTypeRepository = socialMediaTypeRepository ?? throw new ArgumentNullException(nameof(socialMediaTypeRepository));
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

			// Validaciones de negocio - al menos un elemento de cada tipo
			if (!command.Addresses?.Any() == true) throw new ArgumentException("At least one address is required", nameof(command.Addresses));
			if (!command.Emails?.Any() == true) throw new ArgumentException("At least one email is required", nameof(command.Emails));
			if (!command.Phones?.Any() == true) throw new ArgumentException("At least one phone is required", nameof(command.Phones));
			if (!command.SocialMedias?.Any() == true) throw new ArgumentException("At least one social media is required", nameof(command.SocialMedias));
			if (!command.Employees?.Any() == true) throw new ArgumentException("At least one employee is required", nameof(command.Employees));

			var taxId = TaxId.Create(command.TaxId);
			// uniqueness check
			var exists = await _companyRepository.ExistsByTaxIdAsync(taxId.Value, cancellationToken);
			if (exists) throw new InvalidOperationException("A company with the same TaxId already exists");

			// Crear la empresa
			var company = Company.Create(command.Name, taxId);

			// Obtener todos los tipos de contacto
			var addressTypes = await _addressTypeRepository.GetAllAsync(cancellationToken);
			var emailTypes = await _emailTypeRepository.GetAllAsync(cancellationToken);
			var phoneTypes = await _phoneTypeRepository.GetAllAsync(cancellationToken);
			var socialMediaTypes = await _socialMediaTypeRepository.GetAllAsync(cancellationToken);

			// Crear diccionarios para mapear IDs a nombres
			var addressTypeNames = addressTypes.ToDictionary(at => at.Id, at => at.Name);
			var emailTypeNames = emailTypes.ToDictionary(et => et.Id, et => et.Name);
			var phoneTypeNames = phoneTypes.ToDictionary(pt => pt.Id, pt => pt.Name);
			var socialMediaTypeNames = socialMediaTypes.ToDictionary(smt => smt.Id, smt => smt.Name);

			// Agregar direcciones
			foreach (var addressDto in command.Addresses)
			{
				var addressTypeEntity = addressTypes.FirstOrDefault(at => at.Name == addressDto.AddressType);
				if (addressTypeEntity == null)
					throw new ArgumentException($"Address type '{addressDto.AddressType}' not found");
				
				var address = CompanyAddress.Create(company.Id, addressTypeEntity.Id, addressDto.Address, addressDto.IsPrimary);
				company.AddAddress(address);
			}

			// Agregar emails
			foreach (var emailDto in command.Emails)
			{
				var emailTypeEntity = emailTypes.FirstOrDefault(et => et.Name == emailDto.EmailType);
				if (emailTypeEntity == null)
					throw new ArgumentException($"Email type '{emailDto.EmailType}' not found");
				
				var email = Email.Create(emailDto.Email);
				var companyEmail = CompanyEmail.Create(company.Id, emailTypeEntity.Id, email, emailDto.IsPrimary);
				company.AddEmail(companyEmail);
			}

			// Agregar teléfonos
			foreach (var phoneDto in command.Phones)
			{
				var phoneTypeEntity = phoneTypes.FirstOrDefault(pt => pt.Name == phoneDto.PhoneType);
				if (phoneTypeEntity == null)
					throw new ArgumentException($"Phone type '{phoneDto.PhoneType}' not found");
				
				var phone = CompanyPhone.Create(company.Id, phoneTypeEntity.Id, phoneDto.Phone, phoneDto.IsPrimary);
				company.AddPhone(phone);
			}

			// Agregar redes sociales
			foreach (var socialMediaDto in command.SocialMedias)
			{
				var socialMediaTypeEntity = socialMediaTypes.FirstOrDefault(smt => smt.Name == socialMediaDto.SocialMediaType);
				if (socialMediaTypeEntity == null)
					throw new ArgumentException($"Social media type '{socialMediaDto.SocialMediaType}' not found");
				
				var socialMedia = CompanySocialMedia.Create(company.Id, socialMediaTypeEntity.Id, socialMediaDto.Url, socialMediaDto.IsPrimary);
				company.AddSocialMedia(socialMedia);
			}

			// Agregar empleados y crear usuarios automáticamente
			foreach (var employeeDto in command.Employees)
			{
				// Crear usuario del sistema automáticamente para cada empleado
				var user = await CreateUserForEmployee(employeeDto, company.Id, cancellationToken);

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

			return new RegisterCompanyResult
			{
				CompanyId = company.Id,
				Name = company.Name,
				TaxId = company.TaxId.Value,
				Addresses = company.Addresses.Select(a => new CompanyAddressResult(addressTypeNames[a.AddressTypeId], a.Address, a.IsPrimary)).ToList(),
				Emails = company.Emails.Select(e => new CompanyEmailResult(emailTypeNames[e.EmailTypeId], e.Email.Value, e.IsPrimary)).ToList(),
				Phones = company.Phones.Select(p => new CompanyPhoneResult(phoneTypeNames[p.PhoneTypeId], p.Phone, p.IsPrimary)).ToList(),
				SocialMedias = company.SocialMedias.Select(sm => new CompanySocialMediaResult(socialMediaTypeNames[sm.SocialMediaTypeId], sm.Url, sm.IsPrimary)).ToList(),
				Employees = company.Employees.Select(e => new CompanyEmployeeResult(e.FullName, e.Email, e.Phone, e.Position, e.HireDate)).ToList()
			};
		}

		private async Task<User> CreateUserForEmployee(RegisterCompanyEmployeeDto employeeDto, Guid companyId, CancellationToken cancellationToken)
		{
			// Crear el email y verificar que no exista ya un usuario con este email
			var email = Email.Create(employeeDto.Email);
			var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
			if (existingUser != null)
			{
				throw new InvalidOperationException($"A user with email '{employeeDto.Email}' already exists");
			}

			// Parsear el nombre completo
			var nameParts = employeeDto.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
			var firstName = nameParts.Length > 0 ? nameParts[0] : employeeDto.FullName;
			var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User"; // Default lastName if only one name provided

			// Generar contraseña temporal que cumpla con las políticas
			var temporaryPassword = _passwordGenerator.GenerateTemporaryPassword();
			var hashedPassword = HashedPassword.Create(_passwordHasher.HashPassword(temporaryPassword));

			// Crear el usuario
			var user = User.Create(firstName, lastName, email, hashedPassword, companyId, false);
			
			// Guardar el usuario
			await _userRepository.AddAsync(user, cancellationToken);

			return user;
			// TODO: Enviar la contraseña temporal por email al empleado
			// Por ahora, la contraseña se genera pero no se comunica al usuario
			// En una implementación completa, se debería enviar por email o SMS
		}
	}
}
