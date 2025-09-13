using Dualcomp.Auth.Application.Companies;
using Dualcomp.Auth.Application.Companies.GetCompany;
using Dualcomp.Auth.Application.Companies.RegisterCompany;
using Dualcomp.Auth.Application.Services;
using Dualcomp.Auth.Domain.Companies;
using Dualcomp.Auth.Domain.Companies.Repositories;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Dualcomp.Auth.UnitTests.Application.Companies
{
    public class RegisterCompanyCommandHandlerTests
    {
        private readonly Mock<ICompanyRepository> _companyRepositoryMock;
        private readonly Mock<ICompanyContactService> _contactServiceMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailValidationRepository> _emailValidationRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<ICompanySettingsService> _companySettingsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly RegisterCompanyCommandHandler _handler;

        public RegisterCompanyCommandHandlerTests()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();
            _contactServiceMock = new Mock<ICompanyContactService>();
            
            // Setup default ContactTypeNames for the mock
            var defaultContactTypeNames = new ContactTypeNames(
                new Dictionary<Guid, string>(),
                new Dictionary<Guid, string>(),
                new Dictionary<Guid, string>(),
                new Dictionary<Guid, string>()
            );
            
            _contactServiceMock.Setup(x => x.ProcessAllContactsAsync(
                It.IsAny<Company>(), 
                It.IsAny<IEnumerable<dynamic>>(), 
                It.IsAny<IEnumerable<dynamic>>(), 
                It.IsAny<IEnumerable<dynamic>>(), 
                It.IsAny<IEnumerable<dynamic>>(), 
                It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company company, IEnumerable<dynamic> addresses, IEnumerable<dynamic> emails, IEnumerable<dynamic> phones, IEnumerable<dynamic> socialMedias, CancellationToken ct) =>
                {
                    // Simular el procesamiento real agregando contactos a la empresa
                    // Usar un ID temporal ya que la empresa aún no tiene ID asignado
                    var tempCompanyId = Guid.NewGuid();
                    
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
                            Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("info@company.com"), // Email
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
            _contactServiceMock.Setup(x => x.CreateUserForEmployee(
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
                        Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("hashedPassword"), 
                        companyId);
                    return user;
                });
            
            // Setup Build*Results mocks to return realistic data
            _contactServiceMock.Setup(x => x.BuildAddressResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns((Company company, Dictionary<Guid, string> typeNames) => 
                {
                    return company.Addresses.Select(a => new CompanyAddressResult(
                        "Principal", // TypeName
                        a.Address, 
                        a.IsPrimary
                    )).ToList();
                });
            _contactServiceMock.Setup(x => x.BuildEmailResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns((Company company, Dictionary<Guid, string> typeNames) => 
                {
                    return company.Emails.Select(e => new CompanyEmailResult(
                        "Principal", // TypeName
                        e.Email.Value, 
                        e.IsPrimary
                    )).ToList();
                });
            _contactServiceMock.Setup(x => x.BuildPhoneResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns((Company company, Dictionary<Guid, string> typeNames) => 
                {
                    return company.Phones.Select(p => new CompanyPhoneResult(
                        "Principal", // TypeName
                        p.Phone, 
                        p.IsPrimary
                    )).ToList();
                });
            _contactServiceMock.Setup(x => x.BuildSocialMediaResults(It.IsAny<Company>(), It.IsAny<Dictionary<Guid, string>>()))
                .Returns((Company company, Dictionary<Guid, string> typeNames) => 
                {
                    return company.SocialMedias.Select(sm => new CompanySocialMediaResult(
                        "Facebook", // TypeName
                        sm.Url, 
                        sm.IsPrimary
                    )).ToList();
                });
            _contactServiceMock.Setup(x => x.BuildEmployeeResults(It.IsAny<Company>()))
                .Returns((Company company) => 
                {
                    return company.Employees.Select(e => new CompanyEmployeeResult(
                        e.FullName, 
                        e.Email, 
                        e.Phone, 
                        e.Position, 
                        e.HireDate
                    )).ToList();
                });
            
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailValidationRepositoryMock = new Mock<IEmailValidationRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _companySettingsServiceMock = new Mock<ICompanySettingsService>();
            _configurationMock = new Mock<IConfiguration>();

            _handler = new RegisterCompanyCommandHandler(
                _companyRepositoryMock.Object,
                _contactServiceMock.Object,
                _userRepositoryMock.Object,
                _emailValidationRepositoryMock.Object,
                _passwordHasherMock.Object,
                _passwordGeneratorMock.Object,
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _emailTemplateServiceMock.Object,
                _companySettingsServiceMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldRegisterCompanyAndSendEmails()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new RegisterCompanyCommand
            {
                Name = "Test Company",
                TaxId = "12345678-9",
                Addresses = new List<RegisterCompanyAddressDto>
                {
                    new() { AddressTypeId = Guid.NewGuid(), Address = "Test Address", IsPrimary = true }
                },
                Emails = new List<RegisterCompanyEmailDto>
                {
                    new() { EmailTypeId = Guid.NewGuid(), Email = "test@company.com", IsPrimary = true }
                },
                Phones = new List<RegisterCompanyPhoneDto>
                {
                    new() { PhoneTypeId = Guid.NewGuid(), Phone = "+56912345678", IsPrimary = true }
                },
                SocialMedias = new List<RegisterCompanySocialMediaDto>
                {
                    new() { SocialMediaTypeId = Guid.NewGuid(), Url = "https://linkedin.com/company/test", IsPrimary = true }
                },
                Employees = new List<RegisterCompanyEmployeeDto>
                {
                    new() { FullName = "John Doe", Email = "john@company.com", Phone = "+56912345678", Position = "Developer" }
                }
            };

            var company = Company.Create(command.Name, TaxId.Create(command.TaxId));
            var user = User.Create("John", "Doe", Email.Create("john@company.com"), HashedPassword.Create("hashedPassword"), companyId);
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            _companyRepositoryMock.Setup(x => x.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _contactServiceMock.Setup(x => x.ProcessAllContactsAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company company, IEnumerable<dynamic> addresses, IEnumerable<dynamic> emails, IEnumerable<dynamic> phones, IEnumerable<dynamic> socialMedias, CancellationToken ct) =>
                {
                    // Simular el procesamiento real agregando contactos a la empresa
                    var tempCompanyId = Guid.NewGuid();
                    
                    foreach (var address in addresses)
                    {
                        var addressEntity = CompanyAddress.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "123 Main St",
                            true
                        );
                        company.AddAddress(addressEntity);
                    }
                    
                    foreach (var email in emails)
                    {
                        var emailEntity = CompanyEmail.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("info@company.com"),
                            true
                        );
                        company.AddEmail(emailEntity);
                    }
                    
                    foreach (var phone in phones)
                    {
                        var phoneEntity = CompanyPhone.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "+1234567890",
                            true
                        );
                        company.AddPhone(phoneEntity);
                    }
                    
                    foreach (var socialMedia in socialMedias)
                    {
                        var socialMediaEntity = CompanySocialMedia.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "https://facebook.com/company",
                            true
                        );
                        company.AddSocialMedia(socialMediaEntity);
                    }
                    
                    return new ContactTypeNames(
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>());
                });
            _contactServiceMock.Setup(x => x.CreateUserForEmployee(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _companyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companySettings);
            _emailValidationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailTemplateServiceMock.Setup(x => x.CreateWelcomeEmailTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new EmailMessage { To = "john@company.com", Subject = "Welcome", Body = "Welcome email" });
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success("Email sent"));
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            Assert.Equal("123456789", result.TaxId); // TaxId se normaliza removiendo caracteres especiales

            // Verify that email services were called
            _companySettingsServiceMock.Verify(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()), Times.Once);
            _emailValidationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()), Times.Once);
            _emailTemplateServiceMock.Verify(x => x.CreateWelcomeEmailTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithEmailServiceFailure_ShouldStillRegisterCompany()
        {
            // Arrange
            var command = new RegisterCompanyCommand
            {
                Name = "Test Company",
                TaxId = "12345678-9",
                Addresses = new List<RegisterCompanyAddressDto>
                {
                    new() { AddressTypeId = Guid.NewGuid(), Address = "Test Address", IsPrimary = true }
                },
                Emails = new List<RegisterCompanyEmailDto>
                {
                    new() { EmailTypeId = Guid.NewGuid(), Email = "test@company.com", IsPrimary = true }
                },
                Phones = new List<RegisterCompanyPhoneDto>
                {
                    new() { PhoneTypeId = Guid.NewGuid(), Phone = "+56912345678", IsPrimary = true }
                },
                SocialMedias = new List<RegisterCompanySocialMediaDto>
                {
                    new() { SocialMediaTypeId = Guid.NewGuid(), Url = "https://linkedin.com/company/test", IsPrimary = true }
                },
                Employees = new List<RegisterCompanyEmployeeDto>
                {
                    new() { FullName = "John Doe", Email = "john@company.com" }
                }
            };

            _companyRepositoryMock.Setup(x => x.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _contactServiceMock.Setup(x => x.ProcessAllContactsAsync(It.IsAny<Company>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<IEnumerable<dynamic>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Company company, IEnumerable<dynamic> addresses, IEnumerable<dynamic> emails, IEnumerable<dynamic> phones, IEnumerable<dynamic> socialMedias, CancellationToken ct) =>
                {
                    // Simular el procesamiento real agregando contactos a la empresa
                    var tempCompanyId = Guid.NewGuid();
                    
                    foreach (var address in addresses)
                    {
                        var addressEntity = CompanyAddress.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "123 Main St",
                            true
                        );
                        company.AddAddress(addressEntity);
                    }
                    
                    foreach (var email in emails)
                    {
                        var emailEntity = CompanyEmail.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("info@company.com"),
                            true
                        );
                        company.AddEmail(emailEntity);
                    }
                    
                    foreach (var phone in phones)
                    {
                        var phoneEntity = CompanyPhone.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "+1234567890",
                            true
                        );
                        company.AddPhone(phoneEntity);
                    }
                    
                    foreach (var socialMedia in socialMedias)
                    {
                        var socialMediaEntity = CompanySocialMedia.Create(
                            tempCompanyId,
                            Guid.NewGuid(),
                            "https://facebook.com/company",
                            true
                        );
                        company.AddSocialMedia(socialMediaEntity);
                    }
                    
                    return new ContactTypeNames(
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>(),
                        new Dictionary<Guid, string>());
                });
            _contactServiceMock.Setup(x => x.CreateUserForEmployee(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(User.Create("John", "Doe", Email.Create("john@company.com"), HashedPassword.Create("hashedPassword")));
            _companyRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SMTP configuration error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(command.Name, result.Name);
            // Company registration should succeed even if email fails
        }

        [Fact]
        public async Task Handle_WithInvalidCommand_ShouldThrowException()
        {
            // Arrange
            var command = new RegisterCompanyCommand
            {
                Name = "", // Invalid name
                TaxId = "12345678-9",
                Addresses = new List<RegisterCompanyAddressDto>(),
                Emails = new List<RegisterCompanyEmailDto>(),
                Phones = new List<RegisterCompanyPhoneDto>(),
                SocialMedias = new List<RegisterCompanySocialMediaDto>(),
                Employees = new List<RegisterCompanyEmployeeDto>()
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithExistingTaxId_ShouldThrowException()
        {
            // Arrange
            var command = new RegisterCompanyCommand
            {
                Name = "Test Company",
                TaxId = "12345678-9",
                Addresses = new List<RegisterCompanyAddressDto>
                {
                    new() { AddressTypeId = Guid.NewGuid(), Address = "123 Main St", IsPrimary = true }
                },
                Emails = new List<RegisterCompanyEmailDto>
                {
                    new() { EmailTypeId = Guid.NewGuid(), Email = "info@company.com", IsPrimary = true }
                },
                Phones = new List<RegisterCompanyPhoneDto>
                {
                    new() { PhoneTypeId = Guid.NewGuid(), Phone = "+1234567890", IsPrimary = true }
                },
                SocialMedias = new List<RegisterCompanySocialMediaDto>
                {
                    new() { SocialMediaTypeId = Guid.NewGuid(), Url = "https://facebook.com/company", IsPrimary = true }
                },
                Employees = new List<RegisterCompanyEmployeeDto>
                {
                    new() { FullName = "John Doe", Email = "john@company.com" }
                }
            };

            _companyRepositoryMock.Setup(x => x.ExistsByTaxIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true); // TaxId already exists

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
