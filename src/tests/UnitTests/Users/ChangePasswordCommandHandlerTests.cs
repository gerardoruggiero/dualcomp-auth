using Moq;
using Dualcomp.Auth.Application.Users.ChangePassword;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Security;
using DualComp.Infraestructure.Data.Persistence;
using Dualcomp.Auth.Domain.Users.Repositories;

namespace Dualcomp.Auth.UnitTests.Users
{
    public class ChangePasswordCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly Mock<IPasswordValidator> _passwordValidatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ChangePasswordCommandHandler _handler;

        public ChangePasswordCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userSessionRepositoryMock = new Mock<IUserSessionRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _passwordValidatorMock = new Mock<IPasswordValidator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new ChangePasswordCommandHandler(
                _userRepositoryMock.Object,
                _userSessionRepositoryMock.Object,
                _passwordHasherMock.Object,
                _passwordValidatorMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("currentHashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new ChangePasswordCommand(userId, "currentPassword", "newPassword123!");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("currentPassword", "currentHashedPassword"))
                .Returns(true);
            _passwordValidatorMock.Setup(x => x.IsValid("newPassword123!", out It.Ref<string>.IsAny))
                .Returns(true);
            _passwordHasherMock.Setup(x => x.VerifyPassword("newPassword123!", "currentHashedPassword"))
                .Returns(false); // Nueva contraseña es diferente
            _passwordHasherMock.Setup(x => x.HashPassword("newPassword123!"))
                .Returns("newHashedPassword");
            _userRepositoryMock.Setup(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _userSessionRepositoryMock.Setup(x => x.DeactivateAllUserSessionsAsync(userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal("Contraseña actualizada exitosamente. Debe iniciar sesión nuevamente.", result.Message);

            _userRepositoryMock.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _passwordHasherMock.Verify(x => x.VerifyPassword("currentPassword", "currentHashedPassword"), Times.Once);
            _passwordValidatorMock.Verify(x => x.IsValid("newPassword123!", out It.Ref<string>.IsAny), Times.Once);
            _passwordHasherMock.Verify(x => x.HashPassword("newPassword123!"), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _userSessionRepositoryMock.Verify(x => x.DeactivateAllUserSessionsAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidCurrentPassword_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("currentHashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new ChangePasswordCommand(userId, "wrongPassword", "newPassword123!");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("wrongPassword", "currentHashedPassword"))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("La contraseña actual es incorrecta", exception.Message);
        }

        [Fact]
        public async Task Handle_WithInvalidNewPassword_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("currentHashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new ChangePasswordCommand(userId, "currentPassword", "weak");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("currentPassword", "currentHashedPassword"))
                .Returns(true);
            _passwordValidatorMock.Setup(x => x.IsValid("weak", out It.Ref<string>.IsAny))
                .Returns(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            // El mensaje puede variar, pero debe ser una ArgumentException
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task Handle_WithSamePassword_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = Email.Create("test@example.com");
            var hashedPassword = HashedPassword.Create("currentHashedPassword");
            var user = User.Create("John", "Doe", email, hashedPassword);
            
            var command = new ChangePasswordCommand(userId, "currentPassword", "currentPassword");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _passwordHasherMock.Setup(x => x.VerifyPassword("currentPassword", "currentHashedPassword"))
                .Returns(true);
            _passwordValidatorMock.Setup(x => x.IsValid("currentPassword", out It.Ref<string>.IsAny))
                .Returns(true);
            _passwordHasherMock.Setup(x => x.VerifyPassword("currentPassword", "currentHashedPassword"))
                .Returns(true); // Nueva contraseña es igual a la actual

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("La nueva contraseña debe ser diferente a la actual", exception.Message);
        }

        [Fact]
        public async Task Handle_WithNonExistentUser_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new ChangePasswordCommand(userId, "currentPassword", "newPassword123!");

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _handler.Handle(command, CancellationToken.None));
            
            Assert.Equal("Usuario no encontrado o inactivo", exception.Message);
        }
    }
}
