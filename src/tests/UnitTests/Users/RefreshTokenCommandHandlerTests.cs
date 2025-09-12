using Xunit;
using Moq;
using Dualcomp.Auth.Application.Users.RefreshToken;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Domain.Domain.Common.Results;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.UnitTests.Users
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly JwtSettings _jwtSettings;
        private readonly RefreshTokenCommandHandler _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionRepositoryMock = new Mock<IUserSessionRepository>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _jwtSettings = new JwtSettings
            {
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };

            _handler = new RefreshTokenCommandHandler(
                _userRepositoryMock.Object,
                _userSessionRepositoryMock.Object,
                _jwtTokenServiceMock.Object,
                _jwtSettings,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRefreshToken_ShouldReturnSuccess()
        {
            // Arrange
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            var userId = user.Id;
            
            var session = UserSession.Create(
                userId,
                "oldAccessToken",
                "validRefreshToken",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");

            var command = new RefreshTokenCommand("validRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("validRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _jwtTokenServiceMock.Setup(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()))
                .Returns("newAccessToken");
            _jwtTokenServiceMock.Setup(x => x.GenerateRefreshToken())
                .Returns("newRefreshToken");
            _userSessionRepositoryMock.Setup(x => x.UpdateAsync(session, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("newAccessToken", result.AccessToken);
            Assert.Equal("newRefreshToken", result.RefreshToken);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(email.Value, result.Email);
            Assert.Equal(user.GetFullName(), result.FullName);

            _userSessionRepositoryMock.Verify(x => x.GetByRefreshTokenAsync("validRefreshToken", It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
            _jwtTokenServiceMock.Verify(x => x.GenerateAccessToken(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<Guid>(), It.IsAny<bool>()), Times.Once);
            _jwtTokenServiceMock.Verify(x => x.GenerateRefreshToken(), Times.Once);
            _userSessionRepositoryMock.Verify(x => x.UpdateAsync(session, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidRefreshToken_ShouldThrowException()
        {
            // Arrange
            var command = new RefreshTokenCommand("invalidRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("invalidRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserSession?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Token de actualización inválido o expirado", exception.Message);
        }

        [Fact]
        public async Task Handle_WithExpiredSession_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var session = UserSession.Create(
                userId,
                "oldAccessToken",
                "expiredRefreshToken",
                DateTime.UtcNow.AddDays(-1), // Sesión expirada
                "user-agent",
                "127.0.0.1");

            var command = new RefreshTokenCommand("expiredRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("expiredRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Token de actualización inválido o expirado", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInactiveSession_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var session = UserSession.Create(
                userId,
                "oldAccessToken",
                "inactiveRefreshToken",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");
            session.Deactivate(); // Sesión inactiva

            var command = new RefreshTokenCommand("inactiveRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("inactiveRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Token de actualización inválido o expirado", exception.Message);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var session = UserSession.Create(
                userId,
                "oldAccessToken",
                "validRefreshToken",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");

            var command = new RefreshTokenCommand("validRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("validRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Usuario no encontrado o inactivo", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInactiveUser_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("hashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            user.Deactivate(); // Usuario inactivo

            var session = UserSession.Create(
                userId,
                "oldAccessToken",
                "validRefreshToken",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");

            var command = new RefreshTokenCommand("validRefreshToken", "UserAgent", "127.0.0.1");

            _userSessionRepositoryMock.Setup(x => x.GetByRefreshTokenAsync("validRefreshToken", It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);
            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Usuario no encontrado o inactivo", exception.Message);
        }
    }
}
