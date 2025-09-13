namespace DualComp.Infraestructure.Mail.Models
{
    public class SmtpConfiguration
    {
        public string Server { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = true;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public int Timeout { get; set; } = 30000; // 30 segundos
    }
}

