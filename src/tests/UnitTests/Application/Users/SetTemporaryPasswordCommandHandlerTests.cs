using Xunit;
using Moq;
using Dualcomp.Auth.Application.Users.SetTemporaryPassword;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class SetTemporaryPasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly SetTemporaryPasswordCommandHandler _handler;

        public SetTemporaryPasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _configurationMock = new Mock<IConfiguration>();

            _smtpConfiguration = new SmtpConfiguration
            {
                Server = "smtp.gmail.com",
                Port = 587,
                Username = "test@example.com",
                Password = "password",
                UseSsl = true,
                FromEmail = "noreply@example.com",
                FromName = "Test Company"
            };

            _handler = new SetTemporaryPasswordCommandHandler(
                _userRepositoryMock.Object,
                _passwordHasherMock.Object,
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _emailTemplateServiceMock.Object,
                _smtpConfiguration,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidAdminAndUser_ShouldReturnSuccess()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, companyId, true);
            
            var targetEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("target@example.com");
            var targetHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("targetPassword");
            var targetUser = User.Create("Target", "User", targetEmail, targetHashedPassword, companyId);
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");
            var emailMessage = new EmailMessage
            {
                To = targetUser.Email.Value,
                ToName = targetUser.GetFullName(),
                Subject = "Temporary Password",
                Body = "Test body"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);
            _passwordHasherMock.Setup(x => x.HashPassword("tempPassword123"))
                .Returns("hashedTempPassword");
            _userRepositoryMock.Setup(x => x.UpdateAsync(targetUser, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _emailTemplateServiceMock.Setup(x => x.CreateUserCreatedTemplate(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(emailMessage);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success());
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Contraseña temporal establecida exitosamente", result.Message);
            Assert.Equal(targetUser.Email.Value, result.UserEmail);
            Assert.Equal("tempPassword123", result.TemporaryPassword);
            Assert.True(targetUser.RequiresPasswordChange());
            Assert.Equal("tempPassword123", targetUser.TemporaryPassword);

            _userRepositoryMock.Verify(x => x.UpdateAsync(targetUser, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentAdmin_ShouldReturnFailure()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("No tienes permisos para realizar esta acción", result.Message);
            Assert.Null(result.UserEmail);
            Assert.Null(result.TemporaryPassword);
        }

        [Fact]
        public async Task Handle_WithNonAdminUser_ShouldReturnFailure()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, companyId, false); // Not admin
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
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
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, companyId, true);
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
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
        public async Task Handle_WithDifferentCompanyUsers_ShouldReturnFailure()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var adminCompanyId = Guid.NewGuid();
            var targetCompanyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, adminCompanyId, true);
            
            var targetEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("target@example.com");
            var targetHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("targetPassword");
            var targetUser = User.Create("Target", "User", targetEmail, targetHashedPassword, targetCompanyId);
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("No puedes establecer contraseñas para usuarios de otras empresas", result.Message);
        }

        [Fact]
        public async Task Handle_WithEmptyTemporaryPassword_ShouldGeneratePassword()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, companyId, true);
            
            var targetEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("target@example.com");
            var targetHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("targetPassword");
            var targetUser = User.Create("Target", "User", targetEmail, targetHashedPassword, companyId);
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, ""); // Empty password
            var emailMessage = new EmailMessage
            {
                To = targetUser.Email.Value,
                ToName = targetUser.GetFullName(),
                Subject = "Temporary Password",
                Body = "Test body"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);
            _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                .Returns("hashedGeneratedPassword");
            _userRepositoryMock.Setup(x => x.UpdateAsync(targetUser, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _emailTemplateServiceMock.Setup(x => x.CreateUserCreatedTemplate(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(emailMessage);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success());
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.TemporaryPassword);
            Assert.True(result.TemporaryPassword.Length >= 10); // Generated password should be long enough
            Assert.True(targetUser.RequiresPasswordChange());
        }

        [Fact]
        public async Task Handle_WithEmailSendFailure_ShouldStillReturnSuccess()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var companyId = Guid.NewGuid();
            
            var adminEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("admin@example.com");
            var adminHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("adminPassword");
            var adminUser = User.Create("Admin", "User", adminEmail, adminHashedPassword, companyId, true);
            
            var targetEmail = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("target@example.com");
            var targetHashedPassword = Dualcomp.Auth.Domain.Users.ValueObjects.HashedPassword.Create("targetPassword");
            var targetUser = User.Create("Target", "User", targetEmail, targetHashedPassword, companyId);
            
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUser);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(targetUserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(targetUser);
            _passwordHasherMock.Setup(x => x.HashPassword("tempPassword123"))
                .Returns("hashedTempPassword");
            _userRepositoryMock.Setup(x => x.UpdateAsync(targetUser, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Email service error"));
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess); // Should still succeed even if email fails
            Assert.Equal("tempPassword123", result.TemporaryPassword);
        }

        [Fact]
        public async Task Handle_WithException_ShouldReturnFailure()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var targetUserId = Guid.NewGuid();
            var command = new SetTemporaryPasswordCommand(targetUserId, adminId, "tempPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(adminId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al establecer contraseña temporal", result.Message);
        }
    }
}
