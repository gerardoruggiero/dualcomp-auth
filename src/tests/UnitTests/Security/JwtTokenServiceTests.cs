using System.Security.Claims;
using DualComp.Infraestructure.Security;

namespace Dualcomp.Auth.UnitTests.Security
{
    public class JwtTokenServiceTests
    {
        private readonly JwtSettings _jwtSettings;
        private readonly JwtTokenService _jwtTokenService;

        public JwtTokenServiceTests()
        {
            _jwtSettings = new JwtSettings
            {
                SecretKey = "your-super-secret-key-that-is-at-least-32-characters-long-for-security",
                Issuer = "DualComp.Auth",
                Audience = "DualComp.Auth.Users",
                AccessTokenExpirationMinutes = 15,
                RefreshTokenExpirationDays = 7
            };
            _jwtTokenService = new JwtTokenService(_jwtSettings);
        }

        [Fact]
        public void GenerateAccessToken_ShouldReturnValidToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var companyId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var isCompanyAdmin = true;

            // Act
            var token = _jwtTokenService.GenerateAccessToken(userId, email, companyId, sessionId, isCompanyAdmin);

            // Assert
            Assert.NotEmpty(token);
            Assert.Contains(".", token); // JWT format: header.payload.signature
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnValidToken()
        {
            // Act
            var token = _jwtTokenService.GenerateRefreshToken();

            // Assert
            Assert.NotEmpty(token);
            Assert.True(token.Length > 50); // Base64 encoded token should be reasonably long
        }

        [Fact]
        public void ValidateToken_WithValidToken_ShouldReturnPrincipal()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var companyId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var isCompanyAdmin = true;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, companyId, sessionId, isCompanyAdmin);

            // Act
            var principal = _jwtTokenService.ValidateToken(token);

            // Assert
            Assert.NotNull(principal);
            Assert.True(principal.Identity?.IsAuthenticated ?? false);
            
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? principal.FindFirst("sub")?.Value;
            Assert.Equal(userId.ToString(), userIdClaim);
            
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            Assert.Equal(email, emailClaim);
            
            var companyIdClaim = principal.FindFirst("companyId")?.Value;
            Assert.Equal(companyId.ToString(), companyIdClaim);
            
            var sessionIdClaim = principal.FindFirst("sessionId")?.Value;
            Assert.Equal(sessionId.ToString(), sessionIdClaim);
            
            var isAdminClaim = principal.FindFirst("isCompanyAdmin")?.Value;
            Assert.Equal("true", isAdminClaim);
        }

        [Fact]
        public void ValidateToken_WithInvalidToken_ShouldReturnNull()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var principal = _jwtTokenService.ValidateToken(invalidToken);

            // Assert
            Assert.Null(principal);
        }

        [Fact]
        public void ValidateToken_WithEmptyToken_ShouldReturnNull()
        {
            // Arrange
            var emptyToken = "";

            // Act
            var principal = _jwtTokenService.ValidateToken(emptyToken);

            // Assert
            Assert.Null(principal);
        }

        [Fact]
        public void GetTokenExpiration_WithValidToken_ShouldReturnExpirationDate()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var email = "test@example.com";
            var companyId = Guid.NewGuid();
            var sessionId = Guid.NewGuid();
            var isCompanyAdmin = true;

            var token = _jwtTokenService.GenerateAccessToken(userId, email, companyId, sessionId, isCompanyAdmin);

            // Act
            var expiration = _jwtTokenService.GetTokenExpiration(token);

            // Assert
            Assert.True(expiration > DateTime.UtcNow);
            Assert.True(expiration <= DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes + 1)); // Allow 1 minute tolerance
        }

        [Fact]
        public void GetTokenExpiration_WithInvalidToken_ShouldReturnMinValue()
        {
            // Arrange
            var invalidToken = "invalid.token.here";

            // Act
            var expiration = _jwtTokenService.GetTokenExpiration(invalidToken);

            // Assert
            Assert.Equal(DateTime.MinValue, expiration);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldGenerateUniqueTokens()
        {
            // Act
            var token1 = _jwtTokenService.GenerateRefreshToken();
            var token2 = _jwtTokenService.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(token1, token2);
        }
    }
}
