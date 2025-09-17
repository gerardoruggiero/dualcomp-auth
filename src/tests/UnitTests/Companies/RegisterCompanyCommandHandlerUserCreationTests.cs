using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Companies;

public class RegisterCompanyCommandHandlerUserCreationTests
{
	private static (RegisterCompanyCommandHandler handler, Mock<ICompanyRepository> mockCompanyRepo, Mock<IUserRepository> mockUserRepo, Mock<IPasswordHasher> mockPasswordHasher, Mock<IPasswordGenerator> mockPasswordGenerator, Mock<IUnitOfWork> mockUow, AddressTypeEntity principalAddressType, EmailTypeEntity principalEmailType, PhoneTypeEntity principalPhoneType, SocialMediaTypeEntity facebookSocialMediaType) CreateSut()
	{
		var mockCompanyRepo = new Mock<ICompanyRepository>();
		var mockAddressTypeRepo = new Mock<IAddressTypeRepository>();
		var mockEmailTypeRepo = new Mock<IEmailTypeRepository>();
		var mockPhoneTypeRepo = new Mock<IPhoneTypeRepository>();
		var mockSocialMediaTypeRepo = new Mock<ISocialMediaTypeRepository>();
		var mockUserRepo = new Mock<IUserRepository>();
		var mockPasswordHasher = new Mock<IPasswordHasher>();
		var mockPasswordGenerator = new Mock<IPasswordGenerator>();
		var mockUow = new Mock<IUnitOfWork>();

		// Setup default type entities with GetByIdAsync
		var principalAddressType = AddressTypeEntity.Create("Principal");
		var principalEmailType = EmailTypeEntity.Create("Principal");
		var principalPhoneType = PhoneTypeEntity.Create("Principal");
		var facebookSocialMediaType = SocialMediaTypeEntity.Create("Facebook");
		
		mockAddressTypeRepo.Setup(r => r.GetByIdAsync(principalAddressType.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(principalAddressType);
		mockEmailTypeRepo.Setup(r => r.GetByIdAsync(principalEmailType.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(principalEmailType);
		mockPhoneTypeRepo.Setup(r => r.GetByIdAsync(principalPhoneType.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(principalPhoneType);
		mockSocialMediaTypeRepo.Setup(r => r.GetByIdAsync(facebookSocialMediaType.Id, It.IsAny<CancellationToken>()))
			.ReturnsAsync(facebookSocialMediaType);

		// Setup default password generator behavior
		mockPasswordGenerator.Setup(g => g.GenerateTemporaryPassword())
			.Returns("TempPassword123!");

		// Create CompanyContactService mock
		var contactServiceMock = new Mock<Auth.Application.Companies.ICompanyContactService>();
		
		// Setup default ContactTypeNames for the mock
		var defaultContactTypeNames = new Auth.Application.Companies.ContactTypeNames(
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>()
		);
		
		contactServiceMock.Setup(x => x.ProcessAllContactsForRegistrationAsync(
			It.IsAny<Company>(), 
			It.IsAny<IEnumerable<RegisterCompanyAddressDto>>(), 
			It.IsAny<IEnumerable<RegisterCompanyEmailDto>>(), 
			It.IsAny<IEnumerable<RegisterCompanyPhoneDto>>(), 
			It.IsAny<IEnumerable<RegisterCompanySocialMediaDto>>(), 
			It.IsAny<CancellationToken>()))
			.ReturnsAsync((Company company, IEnumerable<RegisterCompanyAddressDto> addresses, IEnumerable<RegisterCompanyEmailDto> emails, IEnumerable<RegisterCompanyPhoneDto> phones, IEnumerable<RegisterCompanySocialMediaDto> socialMedias, CancellationToken ct) =>
			{
				// Simular el procesamiento real agregando contactos a la empresa
				// Usar el ID de la empresa
				var tempCompanyId = company.Id;
				
				foreach (var address in addresses)
				{
					// Agregar dirección mock
					var addressEntity = CompanyAddress.Create(
						tempCompanyId, // CompanyId temporal
						Guid.NewGuid(), // AddressTypeId
						"123 Main St", // Address
						true // IsPrimary
					);
					company.AddAddress(addressEntity);
				}
				
				foreach (var email in emails)
				{
					// Agregar email mock
					var emailEntity = CompanyEmail.Create(
						tempCompanyId, // CompanyId temporal
						Guid.NewGuid(), // EmailTypeId
                        Email.Create("info@company.com"), // Email
						true // IsPrimary
					);
					company.AddEmail(emailEntity);
				}
				
				foreach (var phone in phones)
				{
					// Agregar teléfono mock
					var phoneEntity = CompanyPhone.Create(
						tempCompanyId, // CompanyId temporal
						Guid.NewGuid(), // PhoneTypeId
						"+1234567890", // Phone
						true // IsPrimary
					);
					company.AddPhone(phoneEntity);
				}
				
				foreach (var socialMedia in socialMedias)
				{
					// Agregar red social mock
					var socialMediaEntity = CompanySocialMedia.Create(
						tempCompanyId, // CompanyId temporal
						Guid.NewGuid(), // SocialMediaTypeId
						"https://facebook.com/company", // Url
						true // IsPrimary
					);
					company.AddSocialMedia(socialMediaEntity);
				}
				
				return defaultContactTypeNames;
			});
		
		// Setup CreateUserForEmployee mock to actually call the repository
		contactServiceMock.Setup(x => x.CreateUserForEmployee(
			It.IsAny<string>(), 
			It.IsAny<string>(), 
			It.IsAny<Guid>(), 
			It.IsAny<CancellationToken>()))
			.ReturnsAsync((string fullName, string email, Guid companyId, CancellationToken ct) => 
			{
				// Verificar si el usuario ya existe (como lo hace el servicio real)
				var emailValueObject = Email.Create(email);
				var existingUser = mockUserRepo.Object.GetByEmailAsync(emailValueObject, ct).Result;
				if (existingUser != null)
				{
					throw new InvalidOperationException($"A user with email '{email}' already exists");
				}
				
				// Parsear el nombre completo como lo hace el servicio real
				var nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
				var firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
				var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User";
				
				// Generar contraseña temporal que cumpla con las políticas (como lo hace el servicio real)
				var temporaryPassword = mockPasswordGenerator.Object.GenerateTemporaryPassword();
				var hashedPassword = HashedPassword.Create(mockPasswordHasher.Object.HashPassword(temporaryPassword));
				
				var user = User.Create(
					firstName, 
					lastName, 
					emailValueObject, 
					hashedPassword, 
					companyId, 
					false);
				
				// Simular la llamada real al repositorio
				mockUserRepo.Object.AddAsync(user, ct).Wait();
				return user;
			});
		
		// Setup ProcessEmployeesForRegistrationAsync mock
		contactServiceMock.Setup(x => x.ProcessEmployeesForRegistrationAsync(
			It.IsAny<Company>(), 
			It.IsAny<IEnumerable<RegisterCompanyEmployeeDto>>(), 
			It.IsAny<CancellationToken>()))
			.Returns((Company company, IEnumerable<RegisterCompanyEmployeeDto> employees, CancellationToken ct) =>
			{
				// Simular el procesamiento real agregando empleados a la empresa
				foreach (var employeeDto in employees)
				{
					// Crear usuario para el empleado
					var emailValueObject = Email.Create(employeeDto.Email);
					var existingUser = mockUserRepo.Object.GetByEmailAsync(emailValueObject, ct).Result;
					if (existingUser != null)
					{
						throw new InvalidOperationException($"A user with email '{employeeDto.Email}' already exists");
					}
					
					var nameParts = employeeDto.FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
					var firstName = nameParts.Length > 0 ? nameParts[0] : employeeDto.FullName;
					var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "User";
					
					var temporaryPassword = mockPasswordGenerator.Object.GenerateTemporaryPassword();
					var hashedPassword = HashedPassword.Create(mockPasswordHasher.Object.HashPassword(temporaryPassword));
					
					var user = User.Create(firstName, lastName, emailValueObject, hashedPassword, company.Id, false);
					mockUserRepo.Object.AddAsync(user, ct).Wait();
					
					// Crear empleado
					var employee = Employee.Create(employeeDto.FullName, employeeDto.Email, employeeDto.Phone, company.Id, employeeDto.Position, employeeDto.HireDate, user);
					company.AddEmployee(employee);
				}
				return Task.CompletedTask;
			});
		
		// Setup Build*Results mocks to return realistic data
		contactServiceMock.Setup(x => x.BuildAddressResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Addresses.Select(a => new Auth.Application.Companies.GetCompany.CompanyAddressResult(
				a.Id.ToString(), // Id
				"Principal", // TypeName
				a.Address, 
				a.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildEmailResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Emails.Select(e => new Auth.Application.Companies.GetCompany.CompanyEmailResult(
				e.Id.ToString(), // Id
				"Principal", // TypeName
				e.Email.Value, 
				e.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildPhoneResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Phones.Select(p => new Auth.Application.Companies.GetCompany.CompanyPhoneResult(
				p.Id.ToString(), // Id
				"Principal", // TypeName
				p.Phone, 
				p.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildSocialMediaResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.SocialMedias.Select(sm => new Auth.Application.Companies.GetCompany.CompanySocialMediaResult(
				sm.Id.ToString(), // Id
				"Facebook", // TypeName
				sm.Url, 
				sm.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildEmployeeResults(It.IsAny<Company>()))
			.Returns((Company company) => 
			{
			return company.Employees.Select(e => new Auth.Application.Companies.GetCompany.CompanyEmployeeResult(
				e.Id.ToString(), // Id
				e.FullName, 
				e.Email, 
				e.Phone, 
				e.Position, 
				e.HireDate
			)).ToList();
			});

		// Create additional mocks for the new constructor parameters
		var mockEmailValidationRepo = new Mock<IEmailValidationRepository>();
		var mockEmailService = new Mock<DualComp.Infraestructure.Mail.Interfaces.IEmailService>();
		var mockEmailTemplateService = new Mock<DualComp.Infraestructure.Mail.Interfaces.IEmailTemplateService>();
		var mockCompanySettingsService = new Mock<Auth.Application.Services.ICompanySettingsService>();
		
		// Setup CompanySettingsService mock
		var defaultCompanySettings = CompanySettings.Create(
			Guid.NewGuid(), "smtp.gmail.com", 587, "test@example.com", "password",
			true, "noreply@example.com", "Test Company");
		mockCompanySettingsService.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(
			It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(defaultCompanySettings);
		
		var mockConfiguration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
		mockConfiguration.Setup(x => x["ApplicationSettings:BaseUrl"])
			.Returns("https://localhost:5001");

		var handler = new RegisterCompanyCommandHandler(
			mockCompanyRepo.Object,
			contactServiceMock.Object,
			mockUserRepo.Object,
			mockEmailValidationRepo.Object,
			mockPasswordHasher.Object,
			mockPasswordGenerator.Object,
			mockUow.Object,
			mockEmailService.Object,
			mockEmailTemplateService.Object,
			mockCompanySettingsService.Object,
			mockConfiguration.Object);

		return (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType);
	}

	[Fact]
	public async Task Handle_Should_Create_User_Automatically_For_Each_Employee()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mocks
		mockCompanyRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);
		mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");

		var cmd = new RegisterCompanyCommand
		{
			Name = "Test Company",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/test", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto 
				{ 
					FullName = "John Doe", 
					Email = "john@test.com"
				},
				new RegisterCompanyEmployeeDto 
				{ 
					FullName = "Jane Smith", 
					Email = "jane@test.com"
				}
			}
		};

		// Act
		var result = await handler.Handle(cmd, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, result.CompanyId);
		Assert.Equal(2, result.Employees.Count);

		// Verify that user creation was attempted for each employee
		// GetByEmailAsync se llama 2 veces durante la creación + 2 veces durante el envío de emails = 4 veces total
		mockUserRepo.Verify(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Exactly(4));
		mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
		mockPasswordGenerator.Verify(g => g.GenerateTemporaryPassword(), Times.Exactly(2));
		mockPasswordHasher.Verify(h => h.HashPassword("TempPassword123!"), Times.Exactly(2));
	}

	[Fact]
	public async Task Handle_Should_Throw_When_User_Already_Exists()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mocks
		mockCompanyRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		
		var existingUser = User.Create("Existing", "User", Email.Create("john@test.com"), HashedPassword.Create("hash"), null, false);
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(existingUser);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Test Company",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/test", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto 
				{ 
					FullName = "John Doe", 
					Email = "john@test.com"
				}
			}
		};

		// Act & Assert
		var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd, CancellationToken.None));
		Assert.Contains("A user with email 'john@test.com' already exists", exception.Message);
	}

	[Fact]
	public async Task Handle_Should_Parse_FullName_Correctly()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mocks
		mockCompanyRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);
		mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");

		var cmd = new RegisterCompanyCommand
		{
			Name = "Test Company",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/test", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto 
				{ 
					FullName = "John Michael Doe", 
					Email = "john@test.com"
				}
			}
		};

		// Act
		var result = await handler.Handle(cmd, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, result.CompanyId);

		// Verify that user was created with correct name parsing
		mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => 
			u.FirstName == "John" && 
			u.LastName == "Michael Doe" &&
			u.CompanyId == result.CompanyId &&
			u.IsCompanyAdmin == false), It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_Should_Use_Default_LastName_For_Single_Name()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mocks
		mockCompanyRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);
		mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");

		var cmd = new RegisterCompanyCommand
		{
			Name = "Test Company",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/test", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto 
				{ 
					FullName = "John", 
					Email = "john@test.com"
				}
			}
		};

		// Act
		var result = await handler.Handle(cmd, CancellationToken.None);

		// Assert
		Assert.NotEqual(Guid.Empty, result.CompanyId);

		// Verify that user was created with default lastName
		mockUserRepo.Verify(r => r.AddAsync(It.Is<User>(u => 
			u.FirstName == "John" && 
			u.LastName == "User" &&
			u.CompanyId == result.CompanyId &&
			u.IsCompanyAdmin == false), It.IsAny<CancellationToken>()), Times.Once);
	}
}
