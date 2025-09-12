using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.Application.Users.Login;
using Dualcomp.Auth.Application.Users.Logout;
using Dualcomp.Auth.Application.Users.ChangePassword;
using Dualcomp.Auth.Application.Users.RefreshToken;
using Dualcomp.Auth.Application.Abstractions.Messaging;
using Dualcomp.Auth.Domain.Companies.ValueObjects;
using DualComp.Infraestructure.Domain.Domain.Common.Results;
using System.Security.Claims;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ICommandHandler<LoginCommand, LoginResult> _loginHandler;
        private readonly ICommandHandler<LogoutCommand, LogoutResult> _logoutHandler;
        private readonly ICommandHandler<ChangePasswordCommand, ChangePasswordResult> _changePasswordHandler;
        private readonly ICommandHandler<RefreshTokenCommand, RefreshTokenResult> _refreshTokenHandler;

        public AuthController(
            ICommandHandler<LoginCommand, LoginResult> loginHandler,
            ICommandHandler<LogoutCommand, LogoutResult> logoutHandler,
            ICommandHandler<ChangePasswordCommand, ChangePasswordResult> changePasswordHandler,
            ICommandHandler<RefreshTokenCommand, RefreshTokenResult> refreshTokenHandler)
        {
            _loginHandler = loginHandler ?? throw new ArgumentNullException(nameof(loginHandler));
            _logoutHandler = logoutHandler ?? throw new ArgumentNullException(nameof(logoutHandler));
            _changePasswordHandler = changePasswordHandler ?? throw new ArgumentNullException(nameof(changePasswordHandler));
            _refreshTokenHandler = refreshTokenHandler ?? throw new ArgumentNullException(nameof(refreshTokenHandler));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new LoginCommand(
                    Email.Create(request.Email),
                    request.Password,
                    Request.Headers["User-Agent"].FirstOrDefault(),
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                var result = await _loginHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    expiresAt = result.ExpiresAt,
                    user = new
                    {
                        id = result.UserId,
                        email = result.Email,
                        fullName = result.FullName,
                        companyId = result.CompanyId,
                        isCompanyAdmin = result.IsCompanyAdmin
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("logout")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = ExtractTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token no encontrado" });

            try
            {
                var command = new LogoutCommand(token);
                // Usar CancellationToken.None para evitar cancelación prematura en logout
                var result = await _logoutHandler.Handle(command, CancellationToken.None);

                return Ok(new { message = result.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("change-password")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized(new { message = "Usuario no autenticado" });

            try
            {
                var command = new ChangePasswordCommand(
                    userId.Value,
                    request.CurrentPassword,
                    request.NewPassword);

                var result = await _changePasswordHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(new { message = result.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(new { message = ex.Message });
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

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new RefreshTokenCommand(
                    request.RefreshToken,
                    Request.Headers["User-Agent"].FirstOrDefault(),
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                var result = await _refreshTokenHandler.Handle(command, HttpContext.RequestAborted);

                return Ok(new
                {
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken,
                    expiresAt = result.ExpiresAt,
                    user = new
                    {
                        id = result.UserId,
                        email = result.Email,
                        fullName = result.FullName,
                        companyId = result.CompanyId,
                        isCompanyAdmin = result.IsCompanyAdmin
                    }
                });
            }
            catch (UnauthorizedAccessException ex)
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

        private string? ExtractTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public record LoginRequest(string Email, string Password);
    public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
    public record RefreshTokenRequest(string RefreshToken);
}
