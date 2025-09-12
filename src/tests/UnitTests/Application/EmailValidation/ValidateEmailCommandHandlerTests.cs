using Xunit;
using Moq;
using Dualcomp.Auth.Application.EmailValidation.ValidateEmail;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.UnitTests.Application.EmailValidation
{
    public class ValidateEmailCommandHandlerTests
    {
        private readonly Mock<IEmailValidationRepository> _emailValidationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly ValidateEmailCommandHandler _handler;

        public ValidateEmailCommandHandlerTests()
        {
            _emailValidationRepositoryMock = new Mock<IEmailValidationRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new ValidateEmailCommandHandler(
                _emailValidationRepositoryMock.Object,
                _userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidToken_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "valid-token-123";
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.CreateWithDefaultExpiration(userId, token);
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailValidation);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _emailValidationRepositoryMock.Setup(x => x.UpdateAsync(emailValidation, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.Equal("Email validado exitosamente", result.Message);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.Email.Value, result.UserEmail);
            Assert.True(user.IsEmailValidated);
            Assert.True(emailValidation.IsUsed);

            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _emailValidationRepositoryMock.Verify(x => x.UpdateAsync(emailValidation, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentToken_ShouldReturnFailure()
        {
            // Arrange
            var token = "non-existent-token";
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Dualcomp.Auth.Domain.Users.EmailValidation?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Token de validación no encontrado", result.Message);
            Assert.Null(result.UserId);
            Assert.Null(result.UserEmail);
        }

        [Fact]
        public async Task Handle_WithUsedToken_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "used-token-123";
            var emailValidation = Domain.Users.EmailValidation.CreateWithDefaultExpiration(userId, token);
            emailValidation.MarkAsUsed();
            
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailValidation);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("El token de validación ya ha sido utilizado", result.Message);
        }

        [Fact]
        public async Task Handle_WithExpiredToken_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "expired-token-123";
            var expiresAt = DateTime.UtcNow.AddHours(-1); // Expired
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);
            
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailValidation);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("El token de validación ha expirado", result.Message);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "valid-token-123";
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.CreateWithDefaultExpiration(userId, token);
            
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailValidation);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("Usuario no encontrado", result.Message);
        }

        [Fact]
        public async Task Handle_WithAlreadyValidatedEmail_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "valid-token-123";
            var email = Dualcomp.Auth.Domain.Companies.ValueObjects.Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Already validated
            
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.CreateWithDefaultExpiration(userId, token);
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(emailValidation);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Equal("El email ya ha sido validado anteriormente", result.Message);
        }

        [Fact]
        public async Task Handle_WithException_ShouldReturnFailure()
        {
            // Arrange
            var token = "error-token-123";
            var command = new ValidateEmailCommand(token);

            _emailValidationRepositoryMock.Setup(x => x.GetByTokenAsync(token, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.IsSuccess);
            Assert.Contains("Error al validar el email", result.Message);
        }
    }
}
