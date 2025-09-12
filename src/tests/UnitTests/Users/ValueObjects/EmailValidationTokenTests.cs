using Xunit;
using Dualcomp.Auth.Domain.Users.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Users.ValueObjects
{
    public class EmailValidationTokenTests
    {
        [Fact]
        public void Create_WithValidValue_ShouldCreateToken()
        {
            // Arrange
            var tokenValue = "valid-token-123";

            // Act
            var token = Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Create(tokenValue);

            // Assert
            Assert.NotNull(token);
            Assert.Equal(tokenValue, token.Value);
            Assert.True(token.IsValid());
        }

        [Fact]
        public void Create_WithEmptyValue_ShouldThrowException()
        {
            // Arrange
            var tokenValue = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Create(tokenValue));
        }

        [Fact]
        public void Create_WithWhitespaceValue_ShouldThrowException()
        {
            // Arrange
            var tokenValue = "   ";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Create(tokenValue));
        }

        [Fact]
        public void Generate_ShouldCreateValidToken()
        {
            // Act
            var token = Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Generate();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token.Value);
            Assert.True(token.IsValid());
            Assert.True(token.Value.Length >= 10);
        }

        [Fact]
        public void GenerateWithTimestamp_ShouldCreateValidToken()
        {
            // Act
            var token = Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.GenerateWithTimestamp();

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token.Value);
            Assert.True(token.IsValid());
            Assert.True(token.Value.Length >= 10);
            Assert.Contains("_", token.Value); // Should contain timestamp separator
        }

        [Fact]
        public void IsValid_WithValidToken_ShouldReturnTrue()
        {
            // Arrange
            var token = EmailValidationToken.Create("valid-token-123456");

            // Act
            var isValid = token.IsValid();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_WithShortToken_ShouldReturnFalse()
        {
            // Arrange
            var token = EmailValidationToken.Create("short");

            // Act
            var isValid = token.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var tokenValue = "test-token-123";
            var token = Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Create(tokenValue);

            // Act
            var result = token.ToString();

            // Assert
            Assert.Equal(tokenValue, result);
        }

        [Fact]
        public void ImplicitConversion_ShouldWork()
        {
            // Arrange
            var tokenValue = "test-token-456";
            var token = Dualcomp.Auth.Domain.Users.ValueObjects.EmailValidationToken.Create(tokenValue);

            // Act
            string result = token;

            // Assert
            Assert.Equal(tokenValue, result);
        }

        [Fact]
        public void Equality_WithSameValue_ShouldBeEqual()
        {
            // Arrange
            var tokenValue = "same-token-123";
            var token1 = EmailValidationToken.Create(tokenValue);
            var token2 = EmailValidationToken.Create(tokenValue);

            // Act & Assert
            Assert.Equal(token1, token2);
            Assert.True(token1.Equals(token2));
        }

        [Fact]
        public void Equality_WithDifferentValue_ShouldNotBeEqual()
        {
            // Arrange
            var token1 = EmailValidationToken.Create("token-1");
            var token2 = EmailValidationToken.Create("token-2");

            // Act & Assert
            Assert.NotEqual(token1, token2);
            Assert.False(token1.Equals(token2));
        }
    }
}
