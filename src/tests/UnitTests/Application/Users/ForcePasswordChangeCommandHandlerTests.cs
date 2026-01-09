using Moq;
using Dualcomp.Auth.Application.Users.ForcePasswordChange;
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
    public class ForcePasswordChangeCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly JwtSettings _jwtSettings;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly ForcePasswordChangeCommandHandler _handler;

        public ForcePasswordChangeCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionRepositoryMock = new Mock<IUserSessionRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _emailServiceMock = new Mock<IEmailService>();
            _emailTemplateServiceMock = new Mock<IEmailTemplateService>();
            _configurationMock = new Mock<IConfiguration>();

            _jwtSettings = new JwtSettings
            {
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

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

            _handler = new ForcePasswordChangeCommandHandler(
                _userRepositoryMock.Object,
                _userSessionRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenServiceMock.Object,
                _jwtSettings,
                _unitOfWorkMock.Object,
                _emailServiceMock.Object,
                _emailTemplateServiceMock.Object,
                _smtpConfiguration,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidParameters_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword, Guid.NewGuid());
            user.SetTemporaryPassword("temp123");
            
            var command = new ForcePasswordChangeCommand(userId, "temp123", "newPassword123");
            var emailMessage = new EmailMessage
            {
                To = user.Email.Value,
                ToName = user.GetFullName(),
                Subject = "Password Changed",
                Body = "Test body"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.HashPassword("newPassword123"))
                .Returns("newHashedPassword");
            _userSessionRepositoryMock.Setup(x => x.DeactivateAllUserSessionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userSessionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken");
            _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns("accessToken");
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Success());
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Contraseña cambiada exitosamente. Ya puedes iniciar sesión normalmente.", result.Message);
            Assert.Equal("accessToken", result.AccessToken);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.NotNull(result.ExpiresAt);
            Assert.False(user.RequiresPasswordChange());
            Assert.Null(user.TemporaryPassword);

            _userSessionRepositoryMock.Verify(x => x.DeactivateAllUserSessionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _userSessionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ForcePasswordChangeCommand(userId, "temp123", "newPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Usuario no encontrado", result.Message);
            Assert.Null(result.AccessToken);
            Assert.Null(result.RefreshToken);
            Assert.Null(result.ExpiresAt);
        }

        [Fact]
        public async Task Handle_WithUserNotRequiringPasswordChange_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword, Guid.NewGuid());
            // User doesn't require password change
            
            var command = new ForcePasswordChangeCommand(userId, "temp123", "newPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Este usuario no requiere cambio de contraseña", result.Message);
        }

        [Fact]
        public async Task Handle_WithInvalidTemporaryPassword_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword, Guid.NewGuid());
            user.SetTemporaryPassword("correctTemp123");
            
            var command = new ForcePasswordChangeCommand(userId, "wrongTemp123", "newPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Contraseña temporal inválida", result.Message);
        }

        [Fact]
        public async Task Handle_WithEmailSendFailure_ShouldStillReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword, Guid.NewGuid());
            user.SetTemporaryPassword("temp123");
            
            var command = new ForcePasswordChangeCommand(userId, "temp123", "newPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.HashPassword("newPassword123"))
                .Returns("newHashedPassword");
            _userSessionRepositoryMock.Setup(x => x.DeactivateAllUserSessionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userSessionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken");
            _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns("accessToken");
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Email service error"));
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess); // Should still succeed even if email fails
            Assert.Equal("accessToken", result.AccessToken);
            Assert.Equal("refreshToken", result.RefreshToken);
        }

        [Fact]
        public async Task Handle_WithException_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ForcePasswordChangeCommand(userId, "temp123", "newPassword123");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al cambiar la contraseña", result.Message);
        }
    }
}
