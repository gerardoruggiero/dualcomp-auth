using Xunit;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.UnitTests.Security
{
    public class PasswordValidatorTests
    {
        private readonly PasswordValidationSettings _defaultSettings;
        private readonly PasswordValidator _validator;

        public PasswordValidatorTests()
        {
            _defaultSettings = new PasswordValidationSettings
            {
                MinLength = 8,
                RequireUppercase = true,
                RequireLowercase = true,
                RequireDigit = true,
                RequireSpecialCharacter = true,
                ValidationRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]"
            };
            _validator = new PasswordValidator(_defaultSettings);
        }

        [Fact]
        public void IsValid_WithValidPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "ValidPass123!";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }

        [Fact]
        public void IsValid_WithTooShortPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "Short1!";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña debe tener al menos 8 caracteres.", errorMessage);
        }

        [Fact]
        public void IsValid_WithoutUppercase_ShouldReturnFalse()
        {
            // Arrange
            var password = "validpass123!";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña debe contener al menos una letra mayúscula.", errorMessage);
        }

        [Fact]
        public void IsValid_WithoutLowercase_ShouldReturnFalse()
        {
            // Arrange
            var password = "VALIDPASS123!";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña debe contener al menos una letra minúscula.", errorMessage);
        }

        [Fact]
        public void IsValid_WithoutDigit_ShouldReturnFalse()
        {
            // Arrange
            var password = "ValidPass!";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña debe contener al menos un número.", errorMessage);
        }

        [Fact]
        public void IsValid_WithoutSpecialCharacter_ShouldReturnFalse()
        {
            // Arrange
            var password = "ValidPass123";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña debe contener al menos un carácter especial.", errorMessage);
        }

        [Fact]
        public void IsValid_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "";

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña no puede estar vacía.", errorMessage);
        }

        [Fact]
        public void IsValid_WithNullPassword_ShouldReturnFalse()
        {
            // Arrange
            string? password = null;

            // Act
            var result = _validator.IsValid(password, out string errorMessage);

            // Assert
            Assert.False(result);
            Assert.Equal("La contraseña no puede estar vacía.", errorMessage);
        }

        [Fact]
        public void IsValid_WithRelaxedSettings_ShouldAcceptSimplePassword()
        {
            // Arrange
            var relaxedSettings = new PasswordValidationSettings
            {
                MinLength = 6,
                RequireUppercase = false,
                RequireLowercase = true,
                RequireDigit = true,
                RequireSpecialCharacter = false,
                ValidationRegex = ""
            };
            var relaxedValidator = new PasswordValidator(relaxedSettings);
            var password = "simple123";

            // Act
            var result = relaxedValidator.IsValid(password, out string errorMessage);

            // Assert
            Assert.True(result);
            Assert.Empty(errorMessage);
        }
    }
}
