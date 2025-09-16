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
		
		// Setup CreateUserForEmployee mock
		contactServiceMock.Setup(x => x.CreateUserForEmployee(
			It.IsAny<string>(), 
			It.IsAny<string>(), 
			It.IsAny<Guid>(), 
			It.IsAny<CancellationToken>()))
			.ReturnsAsync((string fullName, string email, Guid companyId, CancellationToken ct) => 
			{
				var user = Dualcomp.Auth.Domain.Users.User.Create(
					fullName.Split(' ')[0], 
					fullName.Split(' ').Length > 1 ? fullName.Split(' ')[1] : "User", 
					Email.Create(email), 
					Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("hashedPassword"), 
					companyId);
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
			}
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
