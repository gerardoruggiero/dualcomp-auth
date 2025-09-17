using Dualcomp.Auth.Application.Users.CreateUser;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Dualcomp.Auth.Application.Services;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailValidationRepository> _emailValidationRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<ICompanySettingsService> _companySettingsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailValidationRepositoryMock = new Mock<IEmailValidationRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _companySettingsServiceMock = new Mock<ICompanySettingsService>();
            _configurationMock = new Mock<IConfiguration>();

            _handler = new CreateUserCommandHandler(
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
        public async Task Handle_WithValidCommand_ShouldCreateUserAndSendEmail()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var command = new CreateUserCommand(
                "John",
                "Doe",
                "john@example.com",
                companyId,
                false,
                Guid.NewGuid());

            var temporaryPassword = "TempPass123!";
            var hashedPassword = "hashedPassword";
            var user = User.Create("John", "Doe", Email.Create("john@example.com"), HashedPassword.Create(hashedPassword), companyId);
            var companySettings = Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); // Email not in use
            _passwordGeneratorMock.Setup(x => x.GenerateTemporaryPassword())
                .Returns(temporaryPassword);
            _passwordHasherMock.Setup(x => x.HashPassword(temporaryPassword))
                .Returns(hashedPassword);
            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailValidationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companySettings);
            _emailTemplateServiceMock.Setup(x => x.CreateWelcomeEmailTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new EmailMessage { To = "john@example.com", Subject = "Welcome", Body = "Welcome email" });
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success("Email sent"));
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
            Assert.Equal("John Doe", result.FullName);
            Assert.Equal(temporaryPassword, result.TemporaryPassword);
            Assert.False(result.IsCompanyAdmin);

            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _emailValidationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingEmail_ShouldThrowException()
        {
            // Arrange
            var command = new CreateUserCommand(
                "John",
                "Doe",
                "existing@example.com",
                Guid.NewGuid());

            var existingUser = User.Create("Existing", "User", Email.Create("existing@example.com"), HashedPassword.Create("hashedPassword"));
            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidFirstName_ShouldThrowException()
        {
            // Arrange
            var command = new CreateUserCommand(
                "", // Invalid first name
                "Doe",
                "john@example.com",
                Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidLastName_ShouldThrowException()
        {
            // Arrange
            var command = new CreateUserCommand(
                "John",
                "", // Invalid last name
                "john@example.com",
                Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var command = new CreateUserCommand(
                "John",
                "Doe",
                "", // Invalid email
                Guid.NewGuid());

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmailServiceFailure_ShouldStillCreateUser()
        {
            // Arrange
            var command = new CreateUserCommand(
                "John",
                "Doe",
                "john@example.com",
                Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _passwordGeneratorMock.Setup(x => x.GenerateTemporaryPassword())
                .Returns("TempPass123!");
            _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("hashedPassword");
            _userRepositoryMock.Setup(x => x.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailValidationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SMTP configuration error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
            // User creation should succeed even if email fails
        }
    }
}
