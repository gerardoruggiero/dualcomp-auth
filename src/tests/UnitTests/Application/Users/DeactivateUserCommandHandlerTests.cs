using Dualcomp.Auth.Application.Users.DeactivateUser;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class DeactivateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly DeactivateUserCommandHandler _handler;

        public DeactivateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeactivateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldDeactivateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeactivateUserCommand(userId, Guid.NewGuid());

            var user = User.Create("John", "Doe", Email.Create("john@example.com"), HashedPassword.Create("hashed"), Guid.NewGuid());
            // User is active by default
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(user.IsActive);
            Assert.False(result.IsActive);
            Assert.Equal(userId, result.UserId);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var command = new DeactivateUserCommand(Guid.NewGuid(), Guid.NewGuid());
            
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
