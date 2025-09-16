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
		var contactServiceMock = new Mock<Dualcomp.Auth.Application.Companies.ICompanyContactService>();
		
		// Setup default ContactTypeNames for the mock
		var defaultContactTypeNames = new Dualcomp.Auth.Application.Companies.ContactTypeNames(
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>(),
			new Dictionary<Guid, string>()
		);
		
		contactServiceMock.Setup(x => x.ProcessAllContactsAsync(
			It.IsAny<Dualcomp.Auth.Domain.Companies.Company>(), 
			It.IsAny<IEnumerable<dynamic>>(), 
			It.IsAny<IEnumerable<dynamic>>(), 
			It.IsAny<IEnumerable<dynamic>>(), 
			It.IsAny<IEnumerable<dynamic>>(), 
			It.IsAny<CancellationToken>()))
			.ReturnsAsync((Dualcomp.Auth.Domain.Companies.Company company, IEnumerable<dynamic> addresses, IEnumerable<dynamic> emails, IEnumerable<dynamic> phones, IEnumerable<dynamic> socialMedias, CancellationToken ct) =>
			{
				// Simular el procesamiento real agregando contactos a la empresa
				// Usar un ID temporal ya que la empresa aún no tiene ID asignado
				var tempCompanyId = Guid.NewGuid();
				
				foreach (var address in addresses)
				{
					// Agregar dirección mock
					var addressEntity = Dualcomp.Auth.Domain.Companies.CompanyAddress.Create(
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
					var emailEntity = Dualcomp.Auth.Domain.Companies.CompanyEmail.Create(
						tempCompanyId, // CompanyId temporal
						Guid.NewGuid(), // EmailTypeId
						Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("info@company.com"), // Email
						true // IsPrimary
					);
					company.AddEmail(emailEntity);
				}
				
				foreach (var phone in phones)
				{
					// Agregar teléfono mock
					var phoneEntity = Dualcomp.Auth.Domain.Companies.CompanyPhone.Create(
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
					var socialMediaEntity = Dualcomp.Auth.Domain.Companies.CompanySocialMedia.Create(
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
				var hashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create(mockPasswordHasher.Object.HashPassword(temporaryPassword));
				
				var user = Dualcomp.Auth.Domain.Users.User.Create(
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
		
		// Setup Build*Results mocks to return realistic data
		contactServiceMock.Setup(x => x.BuildAddressResults(It.IsAny<Dualcomp.Auth.Domain.Companies.Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Dualcomp.Auth.Domain.Companies.Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Addresses.Select(a => new Dualcomp.Auth.Application.Companies.GetCompany.CompanyAddressResult(
				a.Id.ToString(), // Id
				"Principal", // TypeName
				a.Address, 
				a.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildEmailResults(It.IsAny<Dualcomp.Auth.Domain.Companies.Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Dualcomp.Auth.Domain.Companies.Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Emails.Select(e => new Dualcomp.Auth.Application.Companies.GetCompany.CompanyEmailResult(
				e.Id.ToString(), // Id
				"Principal", // TypeName
				e.Email.Value, 
				e.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildPhoneResults(It.IsAny<Dualcomp.Auth.Domain.Companies.Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Dualcomp.Auth.Domain.Companies.Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.Phones.Select(p => new Dualcomp.Auth.Application.Companies.GetCompany.CompanyPhoneResult(
				p.Id.ToString(), // Id
				"Principal", // TypeName
				p.Phone, 
				p.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildSocialMediaResults(It.IsAny<Dualcomp.Auth.Domain.Companies.Company>(), It.IsAny<Dictionary<Guid, string>>()))
			.Returns((Dualcomp.Auth.Domain.Companies.Company company, Dictionary<Guid, string> typeNames) => 
			{
			return company.SocialMedias.Select(sm => new Dualcomp.Auth.Application.Companies.GetCompany.CompanySocialMediaResult(
				sm.Id.ToString(), // Id
				"Facebook", // TypeName
				sm.Url, 
				sm.IsPrimary
			)).ToList();
			});
		contactServiceMock.Setup(x => x.BuildEmployeeResults(It.IsAny<Dualcomp.Auth.Domain.Companies.Company>()))
			.Returns((Dualcomp.Auth.Domain.Companies.Company company) => 
			{
			return company.Employees.Select(e => new Dualcomp.Auth.Application.Companies.GetCompany.CompanyEmployeeResult(
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
		var mockCompanySettingsService = new Mock<Dualcomp.Auth.Application.Services.ICompanySettingsService>();
		
		// Setup CompanySettingsService mock
		var defaultCompanySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
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
