using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Dualcomp.Auth.Application.Users.GetUsers;
using Dualcomp.Auth.Application.Users.CreateUser;
using Dualcomp.Auth.Application.Users.ResetUserPassword;
using Dualcomp.Auth.Application.Users.SetTemporaryPassword;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using System.Security.Claims;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación
    public class UserManagementController : ControllerBase
    {
        private readonly IQueryHandler<GetUsersQuery, GetUsersResult> _getUsersHandler;
        private readonly ICommandHandler<CreateUserCommand, CreateUserResult> _createUserHandler;
        private readonly ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult> _resetUserPasswordHandler;
        private readonly ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult> _setTemporaryPasswordHandler;

        public UserManagementController(
            IQueryHandler<GetUsersQuery, GetUsersResult> getUsersHandler,
            ICommandHandler<CreateUserCommand, CreateUserResult> createUserHandler,
            ICommandHandler<ResetUserPasswordCommand, ResetUserPasswordResult> resetUserPasswordHandler,
            ICommandHandler<SetTemporaryPasswordCommand, SetTemporaryPasswordResult> setTemporaryPasswordHandler)
        {
            _getUsersHandler = getUsersHandler ?? throw new ArgumentNullException(nameof(getUsersHandler));
            _createUserHandler = createUserHandler ?? throw new ArgumentNullException(nameof(createUserHandler));
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
                // Si no se especifica companyId, usar el de la empresa del usuario autenticado
                var userCompanyId = companyId ?? GetCurrentUserCompanyId();
                
                var query = new GetUsersQuery(userCompanyId, page, pageSize, searchTerm);
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
        [Authorize(Roles = "CompanyAdmin")] // Solo administradores pueden crear usuarios
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserId = GetCurrentUserId();
                var currentUserCompanyId = GetCurrentUserCompanyId();

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

        [HttpPost("{userId}/reset-password")]
        [Authorize(Roles = "CompanyAdmin")] // Solo administradores pueden reiniciar contraseñas
        public async Task<IActionResult> ResetUserPassword(Guid userId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();

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
        [Authorize(Roles = "CompanyAdmin")] // Solo administradores pueden establecer contraseñas temporales
        public async Task<IActionResult> SetTemporaryPassword(Guid userId, [FromBody] SetTemporaryPasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var currentUserId = GetCurrentUserId();

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

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }
            return userId;
        }

        private Guid GetCurrentUserCompanyId()
        {
            var companyIdClaim = User.FindFirst("CompanyId")?.Value;
            if (string.IsNullOrEmpty(companyIdClaim) || !Guid.TryParse(companyIdClaim, out var companyId))
            {
                throw new UnauthorizedAccessException("Empresa no identificada");
            }
            return companyId;
        }
    }

    public record CreateUserRequest(
        string FirstName,
        string LastName,
        string Email,
        bool IsCompanyAdmin = false
    );

    public record SetTemporaryPasswordRequest(
        string TemporaryPassword
    );
}
