using System.Text.RegularExpressions;

namespace DualComp.Infraestructure.Security
{
    public class PasswordValidator : IPasswordValidator
    {
        private readonly PasswordValidationSettings _settings;

        public PasswordValidator(PasswordValidationSettings settings) => _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public bool IsValid(string password, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "La contraseña no puede estar vacía.";
                return false;
            }

            if (password.Length < _settings.MinLength)
            {
                errorMessage = $"La contraseña debe tener al menos {_settings.MinLength} caracteres.";
                return false;
            }

            if (_settings.RequireUppercase && !password.Any(char.IsUpper))
            {
                errorMessage = "La contraseña debe contener al menos una letra mayúscula.";
                return false;
            }

            if (_settings.RequireLowercase && !password.Any(char.IsLower))
            {
                errorMessage = "La contraseña debe contener al menos una letra minúscula.";
                return false;
            }

            if (_settings.RequireDigit && !password.Any(char.IsDigit))
            {
                errorMessage = "La contraseña debe contener al menos un número.";
                return false;
            }

            if (_settings.RequireSpecialCharacter && !password.Any(c => !char.IsLetterOrDigit(c)))
            {
                errorMessage = "La contraseña debe contener al menos un carácter especial.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(_settings.ValidationRegex))
            {
                try
                {
                    if (!Regex.IsMatch(password, _settings.ValidationRegex))
                    {
                        errorMessage = "La contraseña no cumple con el formato requerido.";
                        return false;
                    }
                }
                catch (ArgumentException ex)
                {
                    // Log the configuration error but don't fail validation
                    // In production, this should be logged properly
                    errorMessage = "Error en la configuración de validación de contraseñas.";
                    return false;
                }
            }

            return true;
        }
    }

    public class PasswordValidationSettings
    {
        public int MinLength { get; set; } = 8;
        public bool RequireUppercase { get; set; } = true;
        public bool RequireLowercase { get; set; } = true;
        public bool RequireDigit { get; set; } = true;
        public bool RequireSpecialCharacter { get; set; } = true;
        public string? ValidationRegex { get; set; }
    }
}
