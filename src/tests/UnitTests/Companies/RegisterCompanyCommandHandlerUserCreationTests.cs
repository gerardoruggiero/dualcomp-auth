using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Security;
using Moq;

namespace Dualcomp.Auth.UnitTests.Companies;

public class RegisterCompanyCommandHandlerUserCreationTests
{
	private static (RegisterCompanyCommandHandler handler, Mock<ICompanyRepository> mockCompanyRepo, Mock<IUserRepository> mockUserRepo, Mock<IPasswordHasher> mockPasswordHasher, Mock<IPasswordGenerator> mockPasswordGenerator, Mock<IUnitOfWork> mockUow) CreateSut()
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

		// Setup default type entities
		mockAddressTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<AddressTypeEntity> { AddressTypeEntity.Create("Principal") });
		mockEmailTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<EmailTypeEntity> { EmailTypeEntity.Create("Principal") });
		mockPhoneTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<PhoneTypeEntity> { PhoneTypeEntity.Create("Principal") });
		mockSocialMediaTypeRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
			.ReturnsAsync(new List<SocialMediaTypeEntity> { SocialMediaTypeEntity.Create("Facebook") });

		// Setup default password generator behavior
		mockPasswordGenerator.Setup(g => g.GenerateTemporaryPassword())
			.Returns("TempPassword123!");

		var handler = new RegisterCompanyCommandHandler(
			mockCompanyRepo.Object,
			mockAddressTypeRepo.Object,
			mockEmailTypeRepo.Object,
			mockPhoneTypeRepo.Object,
			mockSocialMediaTypeRepo.Object,
			mockUserRepo.Object,
			mockPasswordHasher.Object,
			mockPasswordGenerator.Object,
			mockUow.Object);

		return (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow);
	}

	[Fact]
	public async Task Handle_Should_Create_User_Automatically_For_Each_Employee()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
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
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/test", IsPrimary = true }
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
		mockUserRepo.Verify(r => r.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
		mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
		mockPasswordGenerator.Verify(g => g.GenerateTemporaryPassword(), Times.Exactly(2));
		mockPasswordHasher.Verify(h => h.HashPassword("TempPassword123!"), Times.Exactly(2));
	}

	[Fact]
	public async Task Handle_Should_Throw_When_User_Already_Exists()
	{
		// Arrange
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
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
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/test", IsPrimary = true }
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
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
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
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/test", IsPrimary = true }
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
		var (handler, mockCompanyRepo, mockUserRepo, mockPasswordHasher, mockPasswordGenerator, mockUow) = CreateSut();
		
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
				new RegisterCompanyAddressDto { AddressType = "Principal", Address = "123 Main St", IsPrimary = true }
			},
			Emails = new List<RegisterCompanyEmailDto>
			{
				new RegisterCompanyEmailDto { EmailType = "Principal", Email = "info@test.com", IsPrimary = true }
			},
			Phones = new List<RegisterCompanyPhoneDto>
			{
				new RegisterCompanyPhoneDto { PhoneType = "Principal", Phone = "+1234567890", IsPrimary = true }
			},
			SocialMedias = new List<RegisterCompanySocialMediaDto>
			{
				new RegisterCompanySocialMediaDto { SocialMediaType = "Facebook", Url = "https://facebook.com/test", IsPrimary = true }
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
