using System.Security.Cryptography;

namespace DualComp.Infraestructure.Security
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private readonly PasswordValidationSettings _settings;
        private readonly IPasswordValidator _passwordValidator;

        // Caracteres disponibles para generar contraseñas
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string DigitChars = "0123456789";
        private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

        public PasswordGenerator(PasswordValidationSettings settings, IPasswordValidator passwordValidator)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
        }

        public string GenerateTemporaryPassword()
        {
            var password = string.Empty;
            var attempts = 0;
            const int maxAttempts = 100;

            do
            {
                password = GeneratePassword();
                attempts++;
            } while (!_passwordValidator.IsValid(password, out _) && attempts < maxAttempts);

            if (attempts >= maxAttempts)
            {
                // Fallback: generar una contraseña básica que cumpla los requisitos mínimos
                password = GenerateFallbackPassword();
            }

            return password;
        }

        private string GeneratePassword()
        {
            var password = new List<char>();
            var availableChars = new List<char>();

            // Agregar caracteres requeridos
            if (_settings.RequireLowercase)
            {
                password.Add(GetRandomChar(LowercaseChars));
                availableChars.AddRange(LowercaseChars);
            }

            if (_settings.RequireUppercase)
            {
                password.Add(GetRandomChar(UppercaseChars));
                availableChars.AddRange(UppercaseChars);
            }

            if (_settings.RequireDigit)
            {
                password.Add(GetRandomChar(DigitChars));
                availableChars.AddRange(DigitChars);
            }

            if (_settings.RequireSpecialCharacter)
            {
                password.Add(GetRandomChar(SpecialChars));
                availableChars.AddRange(SpecialChars);
            }

            // Si no hay caracteres disponibles, usar todos los tipos por defecto
            if (availableChars.Count == 0)
            {
                availableChars.AddRange(LowercaseChars);
                availableChars.AddRange(UppercaseChars);
                availableChars.AddRange(DigitChars);
                availableChars.AddRange(SpecialChars);
            }

            // Completar hasta la longitud mínima
            while (password.Count < _settings.MinLength)
            {
                password.Add(GetRandomCharFromList(availableChars));
            }

            // Mezclar los caracteres
            return new string(password.OrderBy(x => RandomNumberGenerator.GetInt32(int.MaxValue)).ToArray());
        }

        private string GenerateFallbackPassword()
        {
            // Contraseña de fallback que siempre cumple los requisitos básicos
            var fallback = "TempPass123!";
            
            // Ajustar longitud si es necesario
            if (fallback.Length < _settings.MinLength)
            {
                var extraLength = _settings.MinLength - fallback.Length;
                var extraChars = new string(Enumerable.Repeat(DigitChars, extraLength)
                    .Select(s => s[RandomNumberGenerator.GetInt32(s.Length)])
                    .ToArray());
                fallback += extraChars;
            }

            return fallback;
        }

        private static char GetRandomChar(string chars)
        {
            if (string.IsNullOrEmpty(chars))
                throw new ArgumentException("Character set cannot be null or empty", nameof(chars));
            
            return chars[RandomNumberGenerator.GetInt32(chars.Length)];
        }

        private static char GetRandomCharFromList(List<char> chars)
        {
            if (chars == null || chars.Count == 0)
                throw new ArgumentException("Character list cannot be null or empty", nameof(chars));
            
            return chars[RandomNumberGenerator.GetInt32(chars.Count)];
        }
    }
}
