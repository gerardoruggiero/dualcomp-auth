using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Companies;

public class RegisterCompanyCommandHandlerTests
{
	private static (RegisterCompanyCommandHandler handler, Mock<ICompanyRepository> mockRepo, Mock<IAddressTypeRepository> mockAddressTypeRepo, Mock<IEmailTypeRepository> mockEmailTypeRepo, Mock<IPhoneTypeRepository> mockPhoneTypeRepo, Mock<ISocialMediaTypeRepository> mockSocialMediaTypeRepo, Mock<IUserRepository> mockUserRepo, Mock<IPasswordHasher> mockPasswordHasher, Mock<IPasswordGenerator> mockPasswordGenerator, Mock<IUnitOfWork> mockUow) CreateSut()
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
		
		// Setup default type entities
		mockAddressTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<AddressTypeEntity> { AddressTypeEntity.Create("Principal") });
		mockEmailTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<EmailTypeEntity> { EmailTypeEntity.Create("Principal") });
		mockPhoneTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<PhoneTypeEntity> { PhoneTypeEntity.Create("Principal") });
		mockSocialMediaTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<SocialMediaTypeEntity> { SocialMediaTypeEntity.Create("Facebook") });
		
		// Setup default user repository behavior
		mockUserRepo.Setup(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((User?)null);
		mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
			.Returns("hashed_password");
		mockPasswordGenerator.Setup(g => g.GenerateTemporaryPassword())
			.Returns("TempPassword123!");
		
		var handler = new RegisterCompanyCommandHandler(mockRepo.Object, mockAddressTypeRepo.Object, mockEmailTypeRepo.Object, mockPhoneTypeRepo.Object, mockSocialMediaTypeRepo.Object, mockUserRepo.Object, mockPasswordHasher.Object, mockPasswordGenerator.Object, mockUow.Object);
		return (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow);
	}

	[Fact]
	public async Task Handle_Should_Create_Company_With_All_Required_Elements()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
		// Setup mock - no existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Acme",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@acme.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/acme", IsPrimary = true }
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
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
		// Setup mock - existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(true);

		var dupCmd = CreateValidCommand("Other", "A-123456");
		await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(dupCmd, CancellationToken.None));
	}

	[Fact]
	public async Task Handle_Should_Throw_On_Invalid_Email()
	{
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
		// Setup mock - no existing company with same TaxId
		mockRepo.Setup(r => r.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(false);

		var cmd = new RegisterCompanyCommand
		{
			Name = "Acme",
			TaxId = "A-123456",
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@acme.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/acme", IsPrimary = true }
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
		var (handler, mockRepo, mockAddressTypeRepo, mockEmailTypeRepo, mockPhoneTypeRepo, mockSocialMediaTypeRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
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

		await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(cmd, CancellationToken.None));
	}

	private static RegisterCompanyCommand CreateValidCommand(string name, string taxId)
	{
		return new RegisterCompanyCommand
		{
			Name = name,
			TaxId = taxId,
			Addresses = new List<RegisterCompanyAddressDto>
			{
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@company.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/company", IsPrimary = true }
			},
			Employees = new List<RegisterCompanyEmployeeDto>
			{
				new RegisterCompanyEmployeeDto { FullName = "John Doe", Email = "john@company.com" }
			}
		};
	}
}
