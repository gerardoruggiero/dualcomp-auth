using Dualcomp.Auth.Application.Users.GetUsers;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.Repositories;
using Dualcomp.Auth.Domain.Users.ValueObjects;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using Moq;
using Xunit;

namespace Dualcomp.Auth.UnitTests.Application.Users
{
    public class GetUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUsersQueryHandler _handler;

        public GetUsersQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUsersQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidQuery_ShouldReturnUsers()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var query = new GetUsersQuery(companyId, 1, 10, "test");

            var users = new List<User>
            {
                User.Create("John", "Doe", Email.Create("john@example.com"), HashedPassword.Create("hashedPassword"), companyId),
                User.Create("Jane", "Smith", Email.Create("jane@example.com"), HashedPassword.Create("hashedPassword"), companyId)
            };

            _userRepositoryMock.Setup(x => x.GetUsersAsync(companyId, 1, 10, "test", It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);
            _userRepositoryMock.Setup(x => x.GetUsersCountAsync(companyId, "test", It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Users.Count);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(1, result.Page);
            Assert.Equal(10, result.PageSize);
            Assert.Equal(1, result.TotalPages);

            _userRepositoryMock.Verify(x => x.GetUsersAsync(companyId, 1, 10, "test", It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(x => x.GetUsersCountAsync(companyId, "test", It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidPage_ShouldThrowException()
        {
            // Arrange
            var query = new GetUsersQuery(null, 0, 10, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidPageSize_ShouldThrowException()
        {
            // Arrange
            var query = new GetUsersQuery(null, 1, 0, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithPageSizeTooLarge_ShouldThrowException()
        {
            // Arrange
            var query = new GetUsersQuery(null, 1, 101, null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithEmptyResults_ShouldReturnEmptyList()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var query = new GetUsersQuery(companyId, 1, 10);

            _userRepositoryMock.Setup(x => x.GetUsersAsync(companyId, 1, 10, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());
            _userRepositoryMock.Setup(x => x.GetUsersCountAsync(companyId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(0);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Users);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);
        }

        [Fact]
        public async Task Handle_WithMultiplePages_ShouldCalculateTotalPagesCorrectly()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var query = new GetUsersQuery(companyId, 1, 5);

            var users = new List<User>
            {
                User.Create("User1", "Test", Email.Create("user1@example.com"), HashedPassword.Create("hashedPassword"), companyId),
                User.Create("User2", "Test", Email.Create("user2@example.com"), HashedPassword.Create("hashedPassword"), companyId),
                User.Create("User3", "Test", Email.Create("user3@example.com"), HashedPassword.Create("hashedPassword"), companyId),
                User.Create("User4", "Test", Email.Create("user4@example.com"), HashedPassword.Create("hashedPassword"), companyId),
                User.Create("User5", "Test", Email.Create("user5@example.com"), HashedPassword.Create("hashedPassword"), companyId)
            };

            _userRepositoryMock.Setup(x => x.GetUsersAsync(companyId, 1, 5, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);
            _userRepositoryMock.Setup(x => x.GetUsersCountAsync(companyId, null, It.IsAny<CancellationToken>()))
                .ReturnsAsync(13); // 13 total users

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.Users.Count);
            Assert.Equal(13, result.TotalCount);
            Assert.Equal(3, result.TotalPages); // Ceiling(13/5) = 3
        }
    }
}
