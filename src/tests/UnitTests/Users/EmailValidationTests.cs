using Xunit;
using Dualcomp.Auth.Domain.Users;
using Dualcomp.Auth.Domain.Users.ValueObjects;

namespace Dualcomp.Auth.UnitTests.Users
{
    public class EmailValidationTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateEmailValidation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-123";
            var expiresAt = DateTime.UtcNow.AddHours(24);

            // Act
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Assert
            Assert.NotNull(emailValidation);
            Assert.Equal(userId, emailValidation.UserId);
            Assert.Equal(token, emailValidation.Token);
            Assert.Equal(expiresAt, emailValidation.ExpiresAt);
            Assert.False(emailValidation.IsUsed);
            Assert.Null(emailValidation.UsedAt);
            Assert.True(emailValidation.IsValid());
        }

        [Fact]
        public void CreateWithDefaultExpiration_ShouldCreateWith24HourExpiration()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-456";

            // Act
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.CreateWithDefaultExpiration(userId, token);

            // Assert
            Assert.NotNull(emailValidation);
            Assert.Equal(userId, emailValidation.UserId);
            Assert.Equal(token, emailValidation.Token);
            Assert.True(emailValidation.ExpiresAt > DateTime.UtcNow.AddHours(23));
            Assert.True(emailValidation.ExpiresAt < DateTime.UtcNow.AddHours(25));
            Assert.False(emailValidation.IsUsed);
            Assert.True(emailValidation.IsValid());
        }

        [Fact]
        public void Create_WithEmptyToken_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "";
            var expiresAt = DateTime.UtcNow.AddHours(24);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt));
        }

        [Fact]
        public void Create_WithWhitespaceToken_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "   ";
            var expiresAt = DateTime.UtcNow.AddHours(24);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt));
        }

        [Fact]
        public void MarkAsUsed_WhenValid_ShouldMarkAsUsed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-789";
            var expiresAt = DateTime.UtcNow.AddHours(24);
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Act
            emailValidation.MarkAsUsed();

            // Assert
            Assert.True(emailValidation.IsUsed);
            Assert.NotNull(emailValidation.UsedAt);
            Assert.True(emailValidation.UsedAt <= DateTime.UtcNow);
            Assert.False(emailValidation.IsValid());
        }

        [Fact]
        public void MarkAsUsed_WhenAlreadyUsed_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-101";
            var expiresAt = DateTime.UtcNow.AddHours(24);
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);
            emailValidation.MarkAsUsed();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => emailValidation.MarkAsUsed());
        }

        [Fact]
        public void MarkAsUsed_WhenExpired_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-202";
            var expiresAt = DateTime.UtcNow.AddHours(-1); // Expired
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => emailValidation.MarkAsUsed());
        }

        [Fact]
        public void IsExpired_WhenExpired_ShouldReturnTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-303";
            var expiresAt = DateTime.UtcNow.AddHours(-1); // Expired
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Act
            var isExpired = emailValidation.IsExpired();

            // Assert
            Assert.True(isExpired);
            Assert.False(emailValidation.IsValid());
        }

        [Fact]
        public void IsExpired_WhenNotExpired_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-404";
            var expiresAt = DateTime.UtcNow.AddHours(1); // Not expired
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Act
            var isExpired = emailValidation.IsExpired();

            // Assert
            Assert.False(isExpired);
            Assert.True(emailValidation.IsValid());
        }

        [Fact]
        public void IsValid_WhenUsed_ShouldReturnFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-505";
            var expiresAt = DateTime.UtcNow.AddHours(24);
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);
            emailValidation.MarkAsUsed();

            // Act
            var isValid = emailValidation.IsValid();

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void GetTimeUntilExpiration_ShouldReturnCorrectTimeSpan()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var token = "test-token-606";
            var expiresAt = DateTime.UtcNow.AddHours(2);
            var emailValidation = Dualcomp.Auth.Domain.Users.EmailValidation.Create(userId, token, expiresAt);

            // Act
            var timeUntilExpiration = emailValidation.GetTimeUntilExpiration();

            // Assert
            Assert.True(timeUntilExpiration.TotalHours > 1.9);
            Assert.True(timeUntilExpiration.TotalHours < 2.1);
        }
    }
}
