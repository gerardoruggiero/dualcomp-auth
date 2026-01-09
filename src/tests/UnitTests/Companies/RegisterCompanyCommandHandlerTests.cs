using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Companies;

public class RegisterCompanyCommandHandlerTests
{
	private static (RegisterCompanyCommandHandler handler, Mock<ICompanyRepository> mockRepo, Mock<IAddressTypeRepository> mockAddressTypeRepo, Mock<IEmailTypeRepository> mockEmailTypeRepo, Mock<IPhoneTypeRepository> mockPhoneTypeRepo, Mock<ISocialMediaTypeRepository> mockSocialMediaTypeRepo, Mock<IUserRepository> mockUserRepo, Mock<IPasswordHasher> mockPasswordHasher, Mock<IPasswordGenerator> mockPasswordGenerator, Mock<IUnitOfWork> mockUow, AddressTypeEntity principalAddressType, EmailTypeEntity principalEmailType, PhoneTypeEntity principalPhoneType, SocialMediaTypeEntity facebookSocialMediaType) CreateSut()
	{
		var mockRepo = new Mock<ICompanyRepository>();
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
		
		// Setup default user repository behavior
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);
		mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");
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
		
		// Setup CreateUserForEmployee mock
		contactServiceMock.Setup(x => x.CreateUserForEmployee(
			It.IsAny<string>(), 
			It.IsAny<string>(), 
			It.IsAny<Guid>(), 
			It.IsAny<CancellationToken>()))
			.ReturnsAsync((string fullName, string email, Guid companyId, CancellationToken ct) => 
			{
				var user = User.Create(
					fullName.Split(' ')[0], 
					fullName.Split(' ').Length > 1 ? fullName.Split(' ')[1] : "User", 
					Email.Create(email),
                    Domain.Users.ValueObjects.HashedPassword.Create("hashedPassword"), 
					companyId);
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
					var user = User.Create(
						employeeDto.FullName.Split(' ')[0], 
						employeeDto.FullName.Split(' ').Length > 1 ? employeeDto.FullName.Split(' ')[1] : "User", 
						Email.Create(employeeDto.Email),
                        Domain.Users.ValueObjects.HashedPassword.Create("hashedPassword"), 
						company.Id);
					
					var employee = Employee.Create(
						employeeDto.FullName, 
						employeeDto.Email, 
						employeeDto.Phone, 
						company.Id, 
						employeeDto.Position, 
						employeeDto.HireDate, 
						user);
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
			mockRepo.Object, 
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
		return (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType);
	}

	[Fact]
	public async Task Handle_Should_Create_Company_With_All_Required_Elements()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mock - no existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Acme",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@acme.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/acme", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto { FullName = "Alice", Email = "alice@acme.com" },
				new RegisterCompanyEmployeeDto { FullName = "Bob", Email = "bob@acme.com", Phone = "+1" }
			}
		};

		var result = await handler.Handle(cmd, CancellationToken.None);

		Assert.NotEqual(Guid.Empty, result.CompanyId);
		Assert.Equal("Acme", result.Name);
		Assert.Equal("A123456", result.TaxId);
		Assert.Single(result.Addresses);
		Assert.Single(result.Emails);
		Assert.Single(result.Phones);
		Assert.Single(result.SocialMedias);
		Assert.Equal(2, result.Employees.Count);

		// Verify repository and unit of work were called
		mockRepo.Verify(r => r.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Once);
		mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
	}

	[Fact]
	public async Task Handle_Should_Throw_On_Duplicate_TaxId()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mock - existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		var dupCmd = CreateValidCommand("Other", "A-123456", principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType);
		await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(dupCmd, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_Should_Throw_On_Invalid_Email()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mock - no existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Acme",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@acme.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/acme", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto { FullName = "Alice", Email = "not-an-email" }
			}
		};

		await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_Should_Throw_On_Missing_Required_Elements()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow, principalAddressType, principalEmailType, principalPhoneType, facebookSocialMediaType) = CreateSut();
		
		// Setup mock - no existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Acme",
			TaxId = "A-123456",
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto { FullName = "Alice", Email = "alice@acme.com" }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>(),
			ModuleIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() }
		};

		await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(cmd, CancellationToken.None));
	}

	private static RegisterCompanyCommand CreateValidCommand(string name, string taxId, AddressTypeEntity principalAddressType, EmailTypeEntity principalEmailType, PhoneTypeEntity principalPhoneType, SocialMediaTypeEntity facebookSocialMediaType)
	{
		return new RegisterCompanyCommand
		{
			Name = name,
			TaxId = taxId,
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressTypeId = principalAddressType.Id, Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailTypeId = principalEmailType.Id, Email = "info@company.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneTypeId = principalPhoneType.Id, Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaTypeId = facebookSocialMediaType.Id, Url = "https://facebook.com/company", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto { FullName = "John Doe", Email = "john@company.com" }
			}
		};
	}
}
