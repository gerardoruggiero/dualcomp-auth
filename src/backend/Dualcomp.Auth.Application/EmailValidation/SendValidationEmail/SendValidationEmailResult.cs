namespace Dualcomp.Auth.Application.EmailValidation.SendValidationEmail
{
    public class SendValidationEmailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ValidationToken { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public static SendValidationEmailResult Success(string validationToken, DateTime expiresAt)
        {
            return new SendValidationEmailResult
            {
                IsSuccess = true,
                Message = "Email de validación enviado exitosamente",
                ValidationToken = validationToken,
                ExpiresAt = expiresAt
            };
        }

        public static SendValidationEmailResult Failure(string message)
        {
            return new SendValidationEmailResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}

