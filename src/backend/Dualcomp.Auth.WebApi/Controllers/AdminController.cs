using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.Application.Services;
using Microsoft.AspNetCore.Authorization;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requiere autenticación
    public class AdminController : ControllerBase
    {
        private readonly EmailValidationCleanupService _cleanupService;

        public AdminController(EmailValidationCleanupService cleanupService)
        {
            _cleanupService = cleanupService ?? throw new ArgumentNullException(nameof(cleanupService));
        }

        [HttpPost("cleanup/email-validation")]
        public async Task<IActionResult> CleanupEmailValidation()
        {
            try
            {
                var result = await _cleanupService.RunCleanupAsync(HttpContext.RequestAborted);

                return Ok(new
                {
                    message = "Limpieza completada exitosamente",
                    expiredTokensDeleted = result.ExpiredTokensDeleted,
                    oldLogsDeleted = result.OldLogsDeleted,
                    executedAt = result.ExecutedAt
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error durante la limpieza: {ex.Message}" });
            }
        }

        [HttpPost("cleanup/expired-tokens")]
        public async Task<IActionResult> CleanupExpiredTokens()
        {
            try
            {
                var count = await _cleanupService.CleanupExpiredTokensAsync(HttpContext.RequestAborted);

                return Ok(new
                {
                    message = $"Limpieza de tokens expirados completada",
                    tokensDeleted = count
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error durante la limpieza de tokens: {ex.Message}" });
            }
        }

        [HttpPost("cleanup/old-logs")]
        public async Task<IActionResult> CleanupOldLogs([FromQuery] int daysOld = 90)
        {
            try
            {
                var count = await _cleanupService.CleanupOldEmailLogsAsync(daysOld, HttpContext.RequestAborted);

                return Ok(new
                {
                    message = $"Limpieza de logs antiguos completada",
                    logsDeleted = count,
                    daysOld = daysOld
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Error durante la limpieza de logs: {ex.Message}" });
            }
        }
    }
}
