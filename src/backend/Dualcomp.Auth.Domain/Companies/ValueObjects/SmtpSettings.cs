using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Companies.ValueObjects
{
    public class SmtpSettings : ValueObject
    {
        public string Server { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; } // Encriptado
        public bool UseSsl { get; private set; }
        public string FromEmail { get; private set; }
        public string FromName { get; private set; }

        private SmtpSettings(
            string server,
            int port,
            string username,
            string password,
            bool useSsl,
            string fromEmail,
            string fromName)
        {
            Server = string.IsNullOrWhiteSpace(server) ? throw new ArgumentException("SMTP Server is required", nameof(server)) : server.Trim();
            Port = port > 0 ? port : throw new ArgumentException("SMTP Port must be greater than 0", nameof(port));
            Username = string.IsNullOrWhiteSpace(username) ? throw new ArgumentException("SMTP Username is required", nameof(username)) : username.Trim();
            Password = string.IsNullOrWhiteSpace(password) ? throw new ArgumentException("SMTP Password is required", nameof(password)) : password;
            UseSsl = useSsl;
            FromEmail = string.IsNullOrWhiteSpace(fromEmail) ? throw new ArgumentException("From Email is required", nameof(fromEmail)) : fromEmail.Trim();
            FromName = string.IsNullOrWhiteSpace(fromName) ? throw new ArgumentException("From Name is required", nameof(fromName)) : fromName.Trim();
        }

        public static SmtpSettings Create(
            string server,
            int port,
            string username,
            string password,
            bool useSsl,
            string fromEmail,
            string fromName)
            => new SmtpSettings(server, port, username, password, useSsl, fromEmail, fromName);

        public static SmtpSettings CreateDefault()
        {
            // Configuración SMTP por defecto para testing
            return new SmtpSettings(
                "smtp.gmail.com",
                587,
                "noreply@dualcomp.com",
                "ENCRYPTED_PASSWORD_HERE",
                true,
                "noreply@dualcomp.com",
                "DualComp CRM"
            );
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Server) &&
                   Port > 0 &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password) &&
                   !string.IsNullOrWhiteSpace(FromEmail) &&
                   !string.IsNullOrWhiteSpace(FromName);
        }

        public string GetConnectionString()
        {
            return $"Server={Server};Port={Port};UseSsl={UseSsl}";
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Server;
            yield return Port;
            yield return Username;
            yield return Password;
            yield return UseSsl;
            yield return FromEmail;
            yield return FromName;
        }
    }
}

