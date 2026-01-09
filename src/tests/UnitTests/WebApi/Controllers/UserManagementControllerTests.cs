using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Application.Users.ActivateUser;
using Dualcomp.Auth.Application.Users.CreateUser;
using Dualcomp.Auth.Application.Users.DeactivateUser;
using Dualcomp.Auth.Application.Users.GetUsers;
using Dualcomp.Auth.Application.Users.ResetUserPassword;
using Dualcomp.Auth.Application.Users.SetTemporaryPassword;
using Dualcomp.Auth.Application.Users.UpdateUser;
using Dualcomp.Auth.WebApi.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Dualcomp.Auth.UnitTests.WebApi.Controllers
{
    public class UserManagementControllerTests
    {
        private readonly Mock<IQueryHandler<GetUsersQuery, GetUsersResult>> _getUsersHandlerMock;
        private readonly Mock<ICommandHandler<CreateUserCommand, CreateUserResult>> _createUserHandlerMock;
        private readonly Mock<ICommandHandler<UpdateUserCommand, UpdateUserResult>> _updateUserHandlerMock;
        private readonly Mock<ICommandHandler<DeactivateUserCommand, DeactivateUserResult>> _deactivateUserHandlerMock;
        private readonly Mock<ICommandHandler<ActivateUserCommand, ActivateUserResult>> _activateUserHandlerMock;
        private readonly Mock<ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult>> _resetUserPasswordHandlerMock;
        private readonly Mock<ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult>> _setTemporaryPasswordHandlerMock;
        private readonly UserManagementController _controller;

        public UserManagementControllerTests()
        {
            _getUsersHandlerMock = new Mock<IQueryHandler<GetUsersQuery, GetUsersResult>>();
            _createUserHandlerMock = new Mock<ICommandHandler<CreateUserCommand, CreateUserResult>>();
            _updateUserHandlerMock = new Mock<ICommandHandler<UpdateUserCommand, UpdateUserResult>>();
            _deactivateUserHandlerMock = new Mock<ICommandHandler<DeactivateUserCommand, DeactivateUserResult>>();
            _activateUserHandlerMock = new Mock<ICommandHandler<ActivateUserCommand, ActivateUserResult>>();
            _resetUserPasswordHandlerMock = new Mock<ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult>>();
            _setTemporaryPasswordHandlerMock = new Mock<ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult>>();

            _controller = new UserManagementController(
                _getUsersHandlerMock.Object,
                _createUserHandlerMock.Object,
                _updateUserHandlerMock.Object,
                _deactivateUserHandlerMock.Object,
                _activateUserHandlerMock.Object,
                _resetUserPasswordHandlerMock.Object,
                _setTemporaryPasswordHandlerMock.Object);
        }

        private void SetupUser(Guid userId, Guid companyId, bool isCompanyAdmin)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim("companyId", companyId.ToString()),
                new Claim("isCompanyAdmin", isCompanyAdmin.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        [Fact]
        public async Task CreateUser_AsNonAdmin_IgnoresRequestCompanyId_AndUsesUserCompanyId()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var currentUserCompanyId = Guid.NewGuid();
            var requestCompanyId = Guid.NewGuid(); // Different company
            
            SetupUser(currentUserId, currentUserCompanyId, isCompanyAdmin: false);

            var request = new CreateUserRequest(
                "John", "Doe", "john@example.com", CompanyId: requestCompanyId, IsCompanyAdmin: false);

            var expectedResult = new CreateUserResult(Guid.NewGuid(), request.Email, "John Doe", "temp", false);
            
            _createUserHandlerMock
                .Setup(x => x.Handle(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult)
                .Callback<CreateUserCommand, CancellationToken>((cmd, ct) =>
                {
                    Assert.Equal(currentUserCompanyId, cmd.CompanyId); // Should NOT be requestCompanyId
                });

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
            _createUserHandlerMock.Verify(x => x.Handle(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateUser_AsAdmin_UsesRequestCompanyId_WhenProvided()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var currentUserCompanyId = Guid.NewGuid();
            var requestCompanyId = Guid.NewGuid(); // Different company
            
            SetupUser(currentUserId, currentUserCompanyId, isCompanyAdmin: true);

            var request = new CreateUserRequest(
                "John", "Doe", "john@example.com", CompanyId: requestCompanyId, IsCompanyAdmin: false);

            var expectedResult = new CreateUserResult(Guid.NewGuid(), request.Email, "John Doe", "temp", false);

            _createUserHandlerMock
                .Setup(x => x.Handle(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult)
                .Callback<CreateUserCommand, CancellationToken>((cmd, ct) =>
                {
                    Assert.Equal(requestCompanyId, cmd.CompanyId); // Should be what was requested
                });

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public async Task CreateUser_AsAdmin_UsesUserCompanyId_WhenRequestCompanyIdIsNull()
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var currentUserCompanyId = Guid.NewGuid();
            
            SetupUser(currentUserId, currentUserCompanyId, isCompanyAdmin: true);

            var request = new CreateUserRequest(
                "John", "Doe", "john@example.com", CompanyId: null, IsCompanyAdmin: false);

            var expectedResult = new CreateUserResult(Guid.NewGuid(), request.Email, "John Doe", "temp", false);

            _createUserHandlerMock
                .Setup(x => x.Handle(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResult)
                .Callback<CreateUserCommand, CancellationToken>((cmd, ct) =>
                {
                    Assert.Equal(currentUserCompanyId, cmd.CompanyId); // Should be admin's company
                });

            // Act
            var result = await _controller.CreateUser(request);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);
        }
    }
}
