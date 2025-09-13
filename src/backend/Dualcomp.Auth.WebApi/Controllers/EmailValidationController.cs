using Microsoft.AspNetCore.Mvc;
using Dualcomp.Auth.Application.EmailValidation.ValidateEmail;
using Dualcomp.Auth.Application.EmailValidation.SendValidationEmail;
using Dualcomp.Auth.Application.Abstractions.Messaging;

namespace Dualcomp.Auth.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailValidationController : ControllerBase
    {
        private readonly ICommandHandler<ValidateEmailCommand, ValidateEmailResult> _validateEmailHandler;
        private readonly ICommandHandler<SendValidationEmailCommand, SendValidationEmailResult> _sendValidationEmailHandler;

        public EmailValidationController(
            ICommandHandler<ValidateEmailCommand, ValidateEmailResult> validateEmailHandler,
            ICommandHandler<SendValidationEmailCommand, SendValidationEmailResult> sendValidationEmailHandler)
        {
            _validateEmailHandler = validateEmailHandler ?? throw new ArgumentNullException(nameof(validateEmailHandler));
            _sendValidationEmailHandler = sendValidationEmailHandler ?? throw new ArgumentNullException(nameof(sendValidationEmailHandler));
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateEmail([FromBody] ValidateEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new ValidateEmailCommand(request.Token);
                var result = await _validateEmailHandler.Handle(command, HttpContext.RequestAborted);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        message = result.Message,
                        userId = result.UserId,
                        userEmail = result.UserEmail
                    });
                }

                return BadRequest(new { message = result.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("send-validation")]
        public async Task<IActionResult> SendValidationEmail([FromBody] SendValidationEmailRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new SendValidationEmailCommand(request.UserId);
                var result = await _sendValidationEmailHandler.Handle(command, HttpContext.RequestAborted);

                if (result.IsSuccess)
                {
                    return Ok(new
                    {
                        message = result.Message,
                        validationToken = result.ValidationToken,
                        expiresAt = result.ExpiresAt
                    });
                }

                return BadRequest(new { message = result.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor" });
            }
        }

        [HttpGet("validate/{token}")]
        public async Task<IActionResult> ValidateEmailByToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest(new { message = "Token es requerido" });

            try
            {
                var command = new ValidateEmailCommand(token);
                var result = await _validateEmailHandler.Handle(command, HttpContext.RequestAborted);

                if (result.IsSuccess)
                {
                    // Para el frontend, podríamos redirigir a una página de éxito
                    return Ok(new
                    {
                        message = result.Message,
                        userId = result.UserId,
                        userEmail = result.UserEmail,
                        success = true
                    });
                }

                return BadRequest(new { message = result.Message, success = false });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Error interno del servidor", success = false });
            }
        }
    }

    public record ValidateEmailRequest(string Token);
    public record SendValidationEmailRequest(Guid UserId);
}

