using Dualcomp.Auth.Application.Users.ResetUserPassword;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Companies.Repositories;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Security;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Dualcomp.Auth.Application.Services;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class ResetUserPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordGenerator> _passwordGeneratorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<ICompanySettingsService> _companySettingsServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly ResetUserPasswordCommandHandler _handler;

        public ResetUserPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordGeneratorMock = new Mock<IPasswordGenerator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _companySettingsServiceMock = new Mock<ICompanySettingsService>();
            _configurationMock = new Mock<IConfiguration>();

            _handler = new ResetUserPasswordCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _passwordGeneratorMock.Object,
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _emailTemplateServiceMock.Object,
                _companySettingsServiceMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldResetPasswordAndSendEmail()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new ResetUserPasswordCommand(targetUserId, adminUserId);

            var adminUser = User.Create("Admin", "User", Email.Create("admin@example.com"), HashedPassword.Create("hashedPassword"), companyId);
            adminUser.SetCompanyAdmin(true);

            var targetUser = User.Create("Target", "User", Email.Create("target@example.com"), HashedPassword.Create("hashedPassword"), companyId);

            var temporaryPassword = "NewTempPass123!";
            var hashedPassword = "newHashedPassword";
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);
            _passwordGeneratorMock.Setup(x => x.GenerateTemporaryPassword())
                .Returns(temporaryPassword);
            _passwordHasherMock.Setup(x => x.HashPassword(temporaryPassword))
                .Returns(hashedPassword);
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(companySettings);
            _emailTemplateServiceMock.Setup(x => x.CreateUserCreatedTemplate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(new EmailMessage { To = "target@example.com", Subject = "Password Reset", Body = "Password reset email" });
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success("Email sent"));
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal(targetUser.Id, result.UserId);
            Assert.Equal("target@example.com", result.Email);
            Assert.Equal("Target User", result.FullName);
            Assert.Equal(temporaryPassword, result.TemporaryPassword);
            Assert.Contains("Contraseña reiniciada exitosamente", result.Message);

            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentAdmin_ShouldReturnFailure()
        {
            // Arrange
            var command = new ResetUserPasswordCommand(Guid.NewGuid(), Guid.NewGuid());

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Administrador no encontrado", result.Message);
        }

        [Fact]
        public async Task Handle_WithNonAdminUser_ShouldReturnFailure()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new ResetUserPasswordCommand(targetUserId, adminUserId);

            var adminUser = User.Create("Admin", "User", Email.Create("admin@example.com"), HashedPassword.Create("hashedPassword"), companyId);
            // adminUser is not set as company admin

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("No tienes permisos para realizar esta acción", result.Message);
        }

        [Fact]
        public async Task Handle_WithNonExistentTargetUser_ShouldReturnFailure()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new ResetUserPasswordCommand(targetUserId, adminUserId);

            var adminUser = User.Create("Admin", "User", Email.Create("admin@example.com"), HashedPassword.Create("hashedPassword"), companyId);
            adminUser.SetCompanyAdmin(true);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Usuario no encontrado", result.Message);
        }

        [Fact]
        public async Task Handle_WithDifferentCompanies_ShouldReturnFailure()
        {
            // Arrange
            var adminCompanyId = Guid.NewGuid();
            var targetCompanyId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new ResetUserPasswordCommand(targetUserId, adminUserId);

            var adminUser = User.Create("Admin", "User", Email.Create("admin@example.com"), HashedPassword.Create("hashedPassword"), adminCompanyId);
            adminUser.SetCompanyAdmin(true);

            var targetUser = User.Create("Target", "User", Email.Create("target@example.com"), HashedPassword.Create("hashedPassword"), targetCompanyId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("No puedes reiniciar la contraseña de usuarios de otras empresas", result.Message);
        }

        [Fact]
        public async Task Handle_WithEmailServiceFailure_ShouldStillResetPassword()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new ResetUserPasswordCommand(targetUserId, adminUserId);

            var adminUser = User.Create("Admin", "User", Email.Create("admin@example.com"), HashedPassword.Create("hashedPassword"), companyId);
            adminUser.SetCompanyAdmin(true);

            var targetUser = User.Create("Target", "User", Email.Create("target@example.com"), HashedPassword.Create("hashedPassword"), companyId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);
            _passwordGeneratorMock.Setup(x => x.GenerateTemporaryPassword())
                .Returns("NewTempPass123!");
            _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("newHashedPassword");
            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _companySettingsServiceMock.Setup(x => x.GetOrCreateDefaultSmtpSettingsAsync(It.IsAny<Guid>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("SMTP configuration error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            // Password reset should succeed even if email fails
        }
    }
}
