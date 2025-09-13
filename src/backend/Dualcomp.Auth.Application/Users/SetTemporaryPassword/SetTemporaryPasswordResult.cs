namespace Dualcomp.Auth.Application.Users.SetTemporaryPassword
{
    public class SetTemporaryPasswordResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public string? TemporaryPassword { get; set; }

        public static SetTemporaryPasswordResult Success(string userEmail, string temporaryPassword)
        {
            return new SetTemporaryPasswordResult
            {
                IsSuccess = true,
                Message = "Contraseña temporal establecida exitosamente",
                UserEmail = userEmail,
                TemporaryPassword = temporaryPassword
            };
        }

        public static SetTemporaryPasswordResult Failure(string message)
        {
            return new SetTemporaryPasswordResult
            {
                IsSuccess = false,
                Message = message
            };
        }
    }
}

