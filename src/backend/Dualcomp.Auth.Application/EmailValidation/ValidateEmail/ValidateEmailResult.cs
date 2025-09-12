namespace Dualcomp.Auth.Application.EmailValidation.ValidateEmail
{
    public class ValidateEmailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? UserEmail { get; set; }

        public static ValidateEmailResult Success(Guid userId, string userEmail)
        {
            return new ValidateEmailResult
            {
                IsSuccess = true,
                Message = "Email validado exitosamente",
                UserId = userId,
                UserEmail = userEmail
            };
        }

        public static ValidateEmailResult Failure(string message)
        {
            return new ValidateEmailResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}
