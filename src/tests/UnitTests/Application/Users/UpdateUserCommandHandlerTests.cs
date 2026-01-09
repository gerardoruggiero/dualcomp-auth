using Dualcomp.Auth.Application.Users.UpdateUser;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using DualComp.Infraestructure.Data.Persistence;
using Moq;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCommand_ShouldUpdateUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(
                userId,
                "Jane",
                "Doe",
                "jane@example.com",
                true, // IsCompanyAdmin
                Guid.NewGuid());

            var user = User.Create("John", "Doe", Email.Create("john@example.com"), HashedPassword.Create("hashed"));
            
            // Simular que el repositorio devuelve el usuario (hack: User.Create genera un ID nuevo, necesitamos asignar el ID esperado o mockear el repositorio para que devuelva este usuario cuando se busque por el ID del comando)
            // Como User.Id es private set y generado en constructor, no podemos setearlo fácilmente sin reflexión o usar un repositorio fake que ignore el ID o mapear el ID del user creado al mock.
            // Mejor opción: Mockear GetByIdAsync para devolver el usuario creado, y asumir que machea.
            // Para ser más precisos, deberíamos usar reflexión para setear el ID si fuera necesario, pero aquí solo importa que GetById devuelva UN usuario.
            // El handler usa user.Id en el resultado, así que el ID devuelto será el del usuario creado, no necesariamente el del comando si no coinciden.
            // Pero en el handler: var user = await _userRepository.GetByIdAsync(request.UserId...);
            // Así que el usuario devuelto DEBE ser el que tiene el ID solicitado.
            
            // Usando reflexión para setear el Id del usuario para que coincida con el comando
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null); // Email not taken by others

            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jane", user.FirstName);
            Assert.Equal("jane@example.com", user.Email.Value);
            Assert.True(user.IsCompanyAdmin);
            Assert.Equal(userId, result.UserId);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenUserNotFound_ShouldThrowException()
        {
            // Arrange
            var command = new UpdateUserCommand(Guid.NewGuid(), "Jane", "Doe", "jane@example.com", false, Guid.NewGuid());
            
            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WhenEmailAlreadyExistsForOtherUser_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherUserId = Guid.NewGuid();
            var email = "taken@example.com";
            var command = new UpdateUserCommand(userId, "Jane", "Doe", email, false, Guid.NewGuid());

            var user = User.Create("John", "Doe", Email.Create("john@example.com"), HashedPassword.Create("hashed"));
            typeof(User).GetProperty("Id")!.SetValue(user, userId);

            var otherUser = User.Create("Other", "User", Email.Create(email), HashedPassword.Create("hashed"));
            typeof(User).GetProperty("Id")!.SetValue(otherUser, otherUserId);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.Is<Email>(e => e.Value == email), It.IsAny<CancellationToken>()))
                .ReturnsAsync(otherUser);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
