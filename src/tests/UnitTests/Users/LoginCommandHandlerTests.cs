using Moq;
using Dualcomp.Auth.Application.Users.Login;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.UnitTests.Users
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly JwtSettings _jwtSettings;
        private readonly LoginCommandHandler _handler;

        public LoginCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionRepositoryMock = new Mock<IUserSessionRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtSettings = new JwtSettings
            {
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

            _handler = new LoginCommandHandler(
                _userRepositoryMock.Object,
                _userSessionRepositoryMock.Object,
                _passwordHasherMock.Object,
                _jwtTokenServiceMock.Object,
                _jwtSettings,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Validate email first
            
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("password123", "hashedPassword"))
                .Returns(true);
            _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns("accessToken");
            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken");
            _userSessionRepositoryMock.Setup(x => x.DeactivateAllUserSessionsAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userSessionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("accessToken", result.AccessToken);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.Email.Value, result.Email);
            Assert.Equal(user.GetFullName(), result.FullName);

            _userSessionRepositoryMock.Verify(x => x.DeactivateAllUserSessionsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
            _userSessionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ShouldThrowException()
        {
            // Arrange
            var email = Email.Create("nonexistent@example.com");
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Credenciales inválidas", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInvalidPassword_ShouldThrowException()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Validate email first
            
            var command = new LoginCommand(email, "wrongpassword", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("wrongpassword", "hashedPassword"))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Credenciales inválidas", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInactiveUser_ShouldThrowException()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.Deactivate();
            
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Credenciales inválidas", exception.Message);
        }

        [Fact]
        public async Task Handle_WithUnvalidatedEmail_ShouldThrowException()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            // Email is not validated by default (IsEmailValidated = false)
            
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("password123", "hashedPassword"))
                .Returns(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Debes validar tu email antes de iniciar sesión. Revisa tu bandeja de entrada.", exception.Message);
        }

        [Fact]
        public async Task Handle_WithUserRequiringPasswordChange_ShouldReturnRequiresPasswordChange()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Validate email first
            user.SetTemporaryPassword("temp123"); // This sets MustChangePassword = true
            
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("password123", "hashedPassword"))
                .Returns(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.RequiresPasswordChange);
            Assert.True(result.IsEmailValidated);
            Assert.Equal(string.Empty, result.AccessToken);
            Assert.Equal(string.Empty, result.RefreshToken);
            Assert.Equal(DateTime.MinValue, result.ExpiresAt);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.Email.Value, result.Email);
            Assert.Equal(user.GetFullName(), result.FullName);

            // Verify that no session was created
            _userSessionRepositoryMock.Verify(x => x.DeactivateAllUserSessionsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            _userSessionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Never);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithValidUserAndValidatedEmail_ShouldReturnSuccess()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.ValidateEmail(); // Validate email
            
            var command = new LoginCommand(email, "password123", "UserAgent", "127.0.0.1");

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("password123", "hashedPassword"))
                .Returns(true);
            _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns("accessToken");
            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("refreshToken");
            _userSessionRepositoryMock.Setup(x => x.DeactivateAllUserSessionsAsync(user.Id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userSessionRepositoryMock.Setup(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.RequiresPasswordChange);
            Assert.True(result.IsEmailValidated);
            Assert.Equal("accessToken", result.AccessToken);
            Assert.Equal("refreshToken", result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(user.Email.Value, result.Email);
            Assert.Equal(user.GetFullName(), result.FullName);

            _userSessionRepositoryMock.Verify(x => x.DeactivateAllUserSessionsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
            _userSessionRepositoryMock.Verify(x => x.AddAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
