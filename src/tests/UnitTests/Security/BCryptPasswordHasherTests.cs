using Xunit;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.UnitTests.Security
{
    public class BCryptPasswordHasherTests
    {
        private readonly BCryptPasswordHasher _hasher;

        public BCryptPasswordHasherTests() => _hasher = new BCryptPasswordHasher();

        [Fact]
        public void HashPassword_ShouldReturnHashedPassword()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hashedPassword = _hasher.HashPassword(password);

            // Assert
            Assert.NotEmpty(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
            Assert.True(hashedPassword.StartsWith("$2a$")); // BCrypt format
        }

        [Fact]
        public void HashPassword_WithSamePassword_ShouldReturnDifferentHashes()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash1 = _hasher.HashPassword(password);
            var hash2 = _hasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // Different salts should produce different hashes
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _hasher.HashPassword(password);

            // Act
            var result = _hasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var wrongPassword = "WrongPassword123!";
            var hashedPassword = _hasher.HashPassword(password);

            // Act
            var result = _hasher.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _hasher.HashPassword(password);

            // Act
            var result = _hasher.VerifyPassword("", hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithNullPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var hashedPassword = _hasher.HashPassword(password);

            // Act
            var result = _hasher.VerifyPassword(null!, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithEmptyHashedPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var result = _hasher.VerifyPassword(password, "");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithNullHashedPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var result = _hasher.VerifyPassword(password, null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HashPassword_WithEmptyPassword_ShouldThrowException()
        {
            // Arrange
            var emptyPassword = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _hasher.HashPassword(emptyPassword));
        }

        [Fact]
        public void HashPassword_WithNullPassword_ShouldThrowException()
        {
            // Arrange
            string? nullPassword = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _hasher.HashPassword(nullPassword!));
        }

        [Fact]
        public void VerifyPassword_WithInvalidHashedPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var invalidHashedPassword = "invalid-hash";

            // Act
            var result = _hasher.VerifyPassword(password, invalidHashedPassword);

            // Assert
            Assert.False(result);
        }
    }
}
