using Xunit;
using Moq;
using Dualcomp.Auth.Application.EmailValidation.SendValidationEmail;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using Microsoft.Extensions.Configuration;

namespace Dualcomp.Auth.UnitTests.Application.EmailValidation
{
    public class SendValidationEmailCommandHandlerTests
    {
        private readonly Mock<IEmailValidationRepository> _emailValidationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly SendValidationEmailCommandHandler _handler;

        public SendValidationEmailCommandHandlerTests()
        {
            _emailValidationRepositoryMock = new Mock<IEmailValidationRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
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

            _handler = new SendValidationEmailCommandHandler(
                _emailValidationRepositoryMock.Object,
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _emailTemplateServiceMock.Object,
                _smtpConfiguration,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidUser_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new SendValidationEmailCommand(userId);
            var emailMessage = new EmailMessage
            {
                To = user.Email.Value,
                ToName = user.GetFullName(),
                Subject = "Validation Email",
                Body = "Test body"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _emailValidationRepositoryMock.Setup(x => x.HasActiveTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _emailValidationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailTemplateServiceMock.Setup(x => x.CreateEmailValidationTemplate(
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
            Assert.Equal("Email de validación enviado exitosamente", result.Message);
            Assert.NotNull(result.ValidationToken);
            Assert.NotNull(result.ExpiresAt);

            _emailValidationRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new SendValidationEmailCommand(userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Usuario no encontrado", result.Message);
            Assert.Null(result.ValidationToken);
            Assert.Null(result.ExpiresAt);
        }

        [Fact]
        public async Task Handle_WithAlreadyValidatedEmail_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Already validated
            
            var command = new SendValidationEmailCommand(userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("El email ya ha sido validado", result.Message);
        }

        [Fact]
        public async Task Handle_WithActiveToken_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new SendValidationEmailCommand(userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _emailValidationRepositoryMock.Setup(x => x.HasActiveTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Ya existe un token de validación activo para este usuario", result.Message);
        }

        [Fact]
        public async Task Handle_WithEmailSendFailure_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new SendValidationEmailCommand(userId);
            var emailMessage = new EmailMessage
            {
                To = user.Email.Value,
                ToName = user.GetFullName(),
                Subject = "Validation Email",
                Body = "Test body"
            };

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _emailValidationRepositoryMock.Setup(x => x.HasActiveTokenAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _emailValidationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailTemplateServiceMock.Setup(x => x.CreateEmailValidationTemplate(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(emailMessage);
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<EmailMessage>(), It.IsAny<SmtpConfiguration>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(EmailResult.Failure("SMTP error"));
            _emailValidationRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _configurationMock.Setup(x => x["ApplicationSettings:BaseUrl"])
                .Returns("https://localhost:5001");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al enviar email", result.Message);

            _emailValidationRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Dualcomp.Auth.Domain.Users.EmailValidation>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithException_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new SendValidationEmailCommand(userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al generar el token de validación", result.Message);
        }
    }
}
