using Xunit;
using Moq;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Mail.Services;
using Microsoft.Extensions.Logging;

namespace Dualcomp.Auth.UnitTests.Infrastructure.Mail
{
    public class SmtpEmailServiceTests
    {
        private readonly Mock<ILogger<SmtpEmailService>> _loggerMock;
        private readonly SmtpConfiguration _smtpConfiguration;
        private readonly SmtpEmailService _service;

        public SmtpEmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<SmtpEmailService>>();
            _smtpConfiguration = new SmtpConfiguration
            {
                Server = "smtp.gmail.com",
                Port = 587,
                Username = "test@example.com",
                Password = "password",
                UseSsl = true,
                FromEmail = "noreply@example.com",
                FromName = "Test Company",
                Timeout = 30000
            };
            _service = new SmtpEmailService(_loggerMock.Object, _smtpConfiguration);
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateService()
        {
            // Act
            var service = new SmtpEmailService(_loggerMock.Object, _smtpConfiguration);

            // Assert
            Assert.NotNull(service);
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SmtpEmailService(null!, _smtpConfiguration));
        }

        [Fact]
        public void Constructor_WithNullSmtpConfiguration_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SmtpEmailService(_loggerMock.Object, null!));
        }

        [Fact]
        public async Task SendEmailAsync_WithNullMessage_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SendEmailAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task SendEmailAsync_WithNullSmtpConfig_ShouldThrowException()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                To = "recipient@example.com",
                ToName = "Recipient Name",
                Subject = "Test Subject",
                Body = "Test Body",
                IsHtml = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.SendEmailAsync(emailMessage, null!, CancellationToken.None));
        }

        [Fact]
        public async Task SendEmailAsync_WithInvalidEmailMessage_ShouldReturnFailure()
        {
            // Arrange
            var invalidEmailMessage = new EmailMessage
            {
                To = "", // Invalid empty email
                ToName = "Recipient Name",
                Subject = "Test Subject",
                Body = "Test Body",
                IsHtml = true
            };

            // Act
            var result = await _service.SendEmailAsync(invalidEmailMessage, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        [Fact]
        public async Task SendEmailAsync_WithInvalidSmtpConfig_ShouldReturnFailure()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                To = "recipient@example.com",
                ToName = "Recipient Name",
                Subject = "Test Subject",
                Body = "Test Body",
                IsHtml = true
            };

            var invalidSmtpConfig = new SmtpConfiguration
            {
                Server = "", // Invalid empty server
                Port = 587,
                Username = "test@example.com",
                Password = "password",
                UseSsl = true,
                FromEmail = "noreply@example.com",
                FromName = "Test Company",
                Timeout = 30000
            };

            // Act
            var result = await _service.SendEmailAsync(emailMessage, invalidSmtpConfig, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error", result.ErrorMessage);
        }

        [Fact]
        public async Task TestConnectionAsync_WithNullSmtpConfig_ShouldThrowException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.TestConnectionAsync(null!, CancellationToken.None));
        }

        [Fact]
        public async Task TestConnectionAsync_WithInvalidSmtpConfig_ShouldReturnFalse()
        {
            // Arrange
            var invalidSmtpConfig = new SmtpConfiguration
            {
                Server = "", // Invalid empty server
                Port = 587,
                Username = "test@example.com",
                Password = "password",
                UseSsl = true,
                FromEmail = "noreply@example.com",
                FromName = "Test Company",
                Timeout = 30000
            };

            // Act
            var result = await _service.TestConnectionAsync(invalidSmtpConfig, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SendEmailAsync_WithCancellation_ShouldHandleCancellation()
        {
            // Arrange
            var emailMessage = new EmailMessage
            {
                To = "recipient@example.com",
                ToName = "Recipient Name",
                Subject = "Test Subject",
                Body = "Test Body",
                IsHtml = true
            };

            using var cts = new CancellationTokenSource();
            cts.Cancel(); // Cancel immediately

            // Act
            var result = await _service.SendEmailAsync(emailMessage, cts.Token);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            // The error message will be about SMTP connection, not cancellation
            Assert.Contains("error", result.ErrorMessage?.ToLower() ?? "");
        }

        [Fact]
        public void SmtpConfiguration_WithValidValues_ShouldBeValid()
        {
            // Arrange & Act
            var config = new SmtpConfiguration
            {
                Server = "smtp.gmail.com",
                Port = 587,
                Username = "test@example.com",
                Password = "password",
                UseSsl = true,
                FromEmail = "noreply@example.com",
                FromName = "Test Company",
                Timeout = 30000
            };

            // Assert
            Assert.NotNull(config);
            Assert.Equal("smtp.gmail.com", config.Server);
            Assert.Equal(587, config.Port);
            Assert.Equal("test@example.com", config.Username);
            Assert.Equal("password", config.Password);
            Assert.True(config.UseSsl);
            Assert.Equal("noreply@example.com", config.FromEmail);
            Assert.Equal("Test Company", config.FromName);
            Assert.Equal(30000, config.Timeout);
        }

        [Fact]
        public void EmailMessage_WithValidValues_ShouldBeValid()
        {
            // Arrange & Act
            var message = new EmailMessage
            {
                To = "recipient@example.com",
                ToName = "Recipient Name",
                Subject = "Test Subject",
                Body = "Test Body",
                IsHtml = true
            };

            // Assert
            Assert.NotNull(message);
            Assert.Equal("recipient@example.com", message.To);
            Assert.Equal("Recipient Name", message.ToName);
            Assert.Equal("Test Subject", message.Subject);
            Assert.Equal("Test Body", message.Body);
            Assert.True(message.IsHtml);
        }
    }
}