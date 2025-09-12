using Dualcomp.Auth.Application.Users.Logout;
using Dualcomp.Auth.Domain.Users;
using DualComp.Infraestructure.Domain.Domain.Common.Results;
using DualComp.Infraestructure.Data.Persistence;
using Moq;
using Xunit;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.UnitTests.Users
{
    public class LogoutCommandHandlerTests
    {
        private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LogoutCommandHandler _handler;

        public LogoutCommandHandlerTests()
        {
            _userSessionRepositoryMock = new Mock<IUserSessionRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new LogoutCommandHandler(_userSessionRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WhenSessionExistsAndIsActive_ShouldDeleteSessionAndReturnSuccess()
        {
            // Arrange
            var accessToken = "valid-access-token";
            var command = new LogoutCommand(accessToken);
            var session = UserSession.Create(
                Guid.NewGuid(),
                accessToken,
                "refresh-token",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");

            _userSessionRepositoryMock
                .Setup(x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            _userSessionRepositoryMock
                .Setup(x => x.DeleteAsync(session, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sesión cerrada exitosamente", result.Message);
            
            _userSessionRepositoryMock.Verify(
                x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _userSessionRepositoryMock.Verify(
                x => x.DeleteAsync(session, It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenSessionDoesNotExist_ShouldReturnSuccessWithMessage()
        {
            // Arrange
            var accessToken = "non-existent-token";
            var command = new LogoutCommand(accessToken);

            _userSessionRepositoryMock
                .Setup(x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((UserSession?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sesión ya cerrada o no encontrada", result.Message);
            
            _userSessionRepositoryMock.Verify(
                x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _userSessionRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenSessionExistsButIsInactive_ShouldReturnSuccessWithMessage()
        {
            // Arrange
            var accessToken = "inactive-session-token";
            var command = new LogoutCommand(accessToken);
            var session = UserSession.Create(
                Guid.NewGuid(),
                accessToken,
                "refresh-token",
                DateTime.UtcNow.AddDays(1),
                "user-agent",
                "127.0.0.1");
            
            // Desactivar la sesión
            session.Deactivate();

            _userSessionRepositoryMock
                .Setup(x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(session);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Sesión ya cerrada o no encontrada", result.Message);
            
            _userSessionRepositoryMock.Verify(
                x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()),
                Times.Once);
            
            _userSessionRepositoryMock.Verify(
                x => x.DeleteAsync(It.IsAny<UserSession>(), It.IsAny<CancellationToken>()),
                Times.Never);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var accessToken = "valid-access-token";
            var command = new LogoutCommand(accessToken);

            _userSessionRepositoryMock
                .Setup(x => x.GetByAccessTokenAsync(accessToken, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _handler.Handle(command, CancellationToken.None));
        }
    }
}
