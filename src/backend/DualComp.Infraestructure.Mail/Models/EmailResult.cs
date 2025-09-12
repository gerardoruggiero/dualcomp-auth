namespace DualComp.Infraestructure.Mail.Models
{
    public class EmailResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public DateTime SentAt { get; set; }

        public static EmailResult Success(string message = "Email enviado exitosamente")
        {
            return new EmailResult
            {
                IsSuccess = true,
                Message = message,
                SentAt = DateTime.UtcNow
            };
        }

        public static EmailResult Failure(string errorMessage)
        {
            return new EmailResult
            {
                IsSuccess = false,
                Message = "Error al enviar email",
                ErrorMessage = errorMessage,
                SentAt = DateTime.UtcNow
            };
        }
    }
}
