using DualComp.Infraestructure.Domain.Domain.Common;

namespace Dualcomp.Auth.Domain.Users.ValueObjects
{
    public class EmailValidationToken : ValueObject
    {
        public string Value { get; private set; }

        private EmailValidationToken(string value)
        {
            Value = string.IsNullOrWhiteSpace(value) 
                ? throw new ArgumentException("Email validation token cannot be empty", nameof(value)) 
                : value.Trim();
        }

        public static EmailValidationToken Create(string value) => new EmailValidationToken(value);

        public static EmailValidationToken Generate()
        {
            // Generar un token único y seguro
            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
            
            return new EmailValidationToken(token);
        }

        public static EmailValidationToken GenerateWithTimestamp()
        {
            // Generar un token con timestamp para mayor seguridad
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var guid = Guid.NewGuid().ToString("N");
            var token = $"{timestamp}_{guid}";
            
            return new EmailValidationToken(token);
        }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Value) && Value.Length >= 10;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        public static implicit operator string(EmailValidationToken token) => token.Value;
    }
}

