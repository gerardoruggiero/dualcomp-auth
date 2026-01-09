using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dualcomp.Auth.Application.Users.GetUsers;
using Dualcomp.Auth.Application.Users.CreateUser;
using Dualcomp.Auth.Application.Users.UpdateUser;
using Dualcomp.Auth.Application.Users.DeactivateUser;
using Dualcomp.Auth.Application.Users.ActivateUser;
using Dualcomp.Auth.Application.Users.ResetUserPassword;
using Dualcomp.Auth.Application.Users.SetTemporaryPassword;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using System.Security.Claims;
using Dualcomp.Auth.WebApi.Extensions;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación
    public class UserManagementController : ControllerBase
    {
        private readonly IQueryHandler<GetUsersQuery, GetUsersResult> _getUsersHandler;
        private readonly ICommandHandler<CreateUserCommand, CreateUserResult> _createUserHandler;
        private readonly ICommandHandler<UpdateUserCommand, UpdateUserResult> _updateUserHandler;
        private readonly ICommandHandler<DeactivateUserCommand, DeactivateUserResult> _deactivateUserHandler;
        private readonly ICommandHandler<ActivateUserCommand, ActivateUserResult> _activateUserHandler;
        private readonly ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult> _resetUserPasswordHandler;
        private readonly ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult> _setTemporaryPasswordHandler;

        public UserManagementController(
            IQueryHandler<GetUsersQuery, GetUsersResult> getUsersHandler,
            ICommandHandler<CreateUserCommand, CreateUserResult> createUserHandler,
            ICommandHandler<UpdateUserCommand, UpdateUserResult> updateUserHandler,
            ICommandHandler<DeactivateUserCommand, DeactivateUserResult> deactivateUserHandler,
            ICommandHandler<ActivateUserCommand, ActivateUserResult> activateUserHandler,
            ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult> resetUserPasswordHandler,
            ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult> setTemporaryPasswordHandler)
        {
            _getUsersHandler = getUsersHandler ?? throw new ArgumentNullException(nameof(getUsersHandler));
            _createUserHandler = createUserHandler ?? throw new ArgumentNullException(nameof(createUserHandler));
            _updateUserHandler = updateUserHandler ?? throw new ArgumentNullException(nameof(updateUserHandler));
            _deactivateUserHandler = deactivateUserHandler ?? throw new ArgumentNullException(nameof(deactivateUserHandler));
            _activateUserHandler = activateUserHandler ?? throw new ArgumentNullException(nameof(activateUserHandler));
            _resetUserPasswordHandler = resetUserPasswordHandler ?? throw new ArgumentNullException(nameof(resetUserPasswordHandler));
            _setTemporaryPasswordHandler = setTemporaryPasswordHandler ?? throw new ArgumentNullException(nameof(setTemporaryPasswordHandler));
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] Guid? companyId = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                GetUsersQuery query;

                if (companyId == null)
                {
                    query = new GetUsersQuery(null, page, pageSize, searchTerm);
                }
                else
                {
                    // Si no se especifica companyId, usar el de la empresa del usuario autenticado
                    var userCompanyId = companyId ?? User.GetCompanyIdOrThrow();
                    query = new GetUsersQuery(userCompanyId, page, pageSize, searchTerm);
                }
                var result = await _getUsersHandler.Handle(query, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost]
        [Authorize] // Solo administradores pueden crear usuarios
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserId = User.GetUserIdOrThrow();
                var currentUserCompanyId = User.GetCompanyIdOrThrow();

                var command = new CreateUserCommand(
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    currentUserCompanyId, // Usar la empresa del usuario autenticado
                    request.IsCompanyAdmin,
                    currentUserId);

                var result = await _createUserHandler.Handle(command, HttpContext.RequestAborted);

                return CreatedAtAction(nameof(GetUsers), new { id = result.UserId }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPut("{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserId = User.GetUserIdOrThrow();

                var command = new UpdateUserCommand(
                    userId,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.IsCompanyAdmin,
                    currentUserId);

                var result = await _updateUserHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpDelete("{userId}")]
        [Authorize]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            try
            {
                var currentUserId = User.GetUserIdOrThrow();

                var command = new DeactivateUserCommand(userId, currentUserId);
                var result = await _deactivateUserHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPatch("{userId}/activate")]
        [Authorize]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            try
            {
                var currentUserId = User.GetUserIdOrThrow();

                var command = new ActivateUserCommand(userId, currentUserId);
                var result = await _activateUserHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("{userId}/reset-password")]
        [Authorize] // Solo administradores pueden reiniciar contraseñas
        public async Task<IActionResult> ResetUserPassword(Guid userId)
        {
            try
            {
                var currentUserId = User.GetUserIdOrThrow();

                var command = new ResetUserPasswordCommand(userId, currentUserId);
                var result = await _resetUserPasswordHandler.Handle(command, HttpContext.RequestAborted);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = result.Message,
                    user = new
                    {
                        id = result.UserId,
                        email = result.Email,
                        fullName = result.FullName
                    }
                });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("{userId}/set-temporary-password")]
        [Authorize] // Solo administradores pueden establecer contraseñas temporales
        public async Task<IActionResult> SetTemporaryPassword(Guid userId, [FromBody] SetTemporaryPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserId = User.GetUserIdOrThrow();

                var command = new SetTemporaryPasswordCommand(
                    userId,
                    currentUserId,
                    request.TemporaryPassword);

                var result = await _setTemporaryPasswordHandler.Handle(command, HttpContext.RequestAborted);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new
                {
                    message = result.Message,
                    user = new
                    {
                        email = result.UserEmail,
                        temporaryPassword = result.TemporaryPassword
                    }
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }
    }

    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        bool IsCompanyAdmin = false
    );

    public record UpdateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        bool IsCompanyAdmin
    );

    public record SetTemporaryPasswordRequest(
        string TemporaryPassword
    );
}
