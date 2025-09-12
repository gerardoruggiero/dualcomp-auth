using Xunit;
using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Mail.Services;

namespace Dualcomp.Auth.UnitTests.Infrastructure.Mail
{
    public class EmailTemplateServiceTests
    {
        private readonly IEmailTemplateService _service;

        public EmailTemplateServiceTests()
        {
            _service = new EmailTemplateService();
        }

        [Fact]
        public void CreateEmailValidationTemplate_WithValidParameters_ShouldCreateValidTemplate()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userName = "John Doe";
            var validationToken = "validation-token-123";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreateEmailValidationTemplate(userEmail, userName, validationToken, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal(userEmail, emailMessage.To);
            Assert.Equal(userName, emailMessage.ToName);
            Assert.Equal("Validación de Email - DualComp CRM", emailMessage.Subject);
            Assert.True(emailMessage.IsHtml);
            Assert.Contains(validationToken, emailMessage.Body);
            Assert.Contains(baseUrl, emailMessage.Body);
            Assert.Contains("Validar Mi Email", emailMessage.Body);
        }

        [Fact]
        public void CreatePasswordResetTemplate_WithValidParameters_ShouldCreateValidTemplate()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userName = "John Doe";
            var resetToken = "reset-token-456";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreatePasswordResetTemplate(userEmail, userName, resetToken, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal(userEmail, emailMessage.To);
            Assert.Equal(userName, emailMessage.ToName);
            Assert.Equal("Restablecer Contraseña - DualComp CRM", emailMessage.Subject);
            Assert.True(emailMessage.IsHtml);
            Assert.Contains(resetToken, emailMessage.Body);
            Assert.Contains(baseUrl, emailMessage.Body);
            Assert.Contains("Restablecer Contraseña", emailMessage.Body);
        }

        [Fact]
        public void CreateCompanyRegistrationTemplate_WithValidParameters_ShouldCreateValidTemplate()
        {
            // Arrange
            var userEmail = "admin@company.com";
            var userName = "Admin User";
            var companyName = "Test Company";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreateCompanyRegistrationTemplate(userEmail, userName, companyName, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal(userEmail, emailMessage.To);
            Assert.Equal(userName, emailMessage.ToName);
            Assert.Equal($"Empresa {companyName} Registrada - DualComp CRM", emailMessage.Subject);
            Assert.True(emailMessage.IsHtml);
            Assert.Contains(companyName, emailMessage.Body);
            Assert.Contains("¡Empresa Registrada Exitosamente!", emailMessage.Body);
        }

        [Fact]
        public void CreateUserCreatedTemplate_WithValidParameters_ShouldCreateValidTemplate()
        {
            // Arrange
            var userEmail = "newuser@example.com";
            var userName = "New User";
            var temporaryPassword = "temp123456";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreateUserCreatedTemplate(userEmail, userName, temporaryPassword, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal(userEmail, emailMessage.To);
            Assert.Equal(userName, emailMessage.ToName);
            Assert.Equal("Nueva Cuenta Creada - DualComp CRM", emailMessage.Subject);
            Assert.True(emailMessage.IsHtml);
            Assert.Contains(temporaryPassword, emailMessage.Body);
            Assert.Contains(baseUrl, emailMessage.Body);
            Assert.Contains("Nueva Cuenta Creada", emailMessage.Body);
        }

        [Fact]
        public void CreateEmailValidationTemplate_WithEmptyParameters_ShouldCreateTemplateWithEmptyValues()
        {
            // Arrange
            var userEmail = "";
            var userName = "";
            var validationToken = "";
            var baseUrl = "";

            // Act
            var emailMessage = _service.CreateEmailValidationTemplate(userEmail, userName, validationToken, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal("", emailMessage.To);
            Assert.Equal("", emailMessage.ToName);
            Assert.Equal("Validación de Email - DualComp CRM", emailMessage.Subject);
            Assert.True(emailMessage.IsHtml);
            Assert.Contains("", emailMessage.Body); // Empty token
        }

        [Fact]
        public void CreateEmailValidationTemplate_WithSpecialCharacters_ShouldEscapeProperly()
        {
            // Arrange
            var userEmail = "test+tag@example.com";
            var userName = "John \"Doe\" & Associates";
            var validationToken = "token-with-special-chars-123";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreateEmailValidationTemplate(userEmail, userName, validationToken, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Equal(userEmail, emailMessage.To);
            Assert.Equal(userName, emailMessage.ToName);
            Assert.Contains(validationToken, emailMessage.Body);
            Assert.Contains(userName, emailMessage.Body);
        }

        [Fact]
        public void CreatePasswordResetTemplate_WithLongBaseUrl_ShouldHandleProperly()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userName = "John Doe";
            var resetToken = "reset-token-789";
            var baseUrl = "https://very-long-domain-name-that-might-cause-issues.example.com:8080/path/to/application";

            // Act
            var emailMessage = _service.CreatePasswordResetTemplate(userEmail, userName, resetToken, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Contains(baseUrl, emailMessage.Body);
            Assert.Contains(resetToken, emailMessage.Body);
        }

        [Fact]
        public void CreateUserCreatedTemplate_WithComplexPassword_ShouldDisplayCorrectly()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userName = "John Doe";
            var temporaryPassword = "Temp@Pass#123$";
            var baseUrl = "https://localhost:5001";

            // Act
            var emailMessage = _service.CreateUserCreatedTemplate(userEmail, userName, temporaryPassword, baseUrl);

            // Assert
            Assert.NotNull(emailMessage);
            Assert.Contains(temporaryPassword, emailMessage.Body);
            Assert.Contains("Contraseña Temporal", emailMessage.Body);
        }

        [Fact]
        public void AllTemplates_ShouldHaveConsistentStyling()
        {
            // Arrange
            var userEmail = "test@example.com";
            var userName = "John Doe";
            var token = "test-token";
            var baseUrl = "https://localhost:5001";

            // Act
            var validationTemplate = _service.CreateEmailValidationTemplate(userEmail, userName, token, baseUrl);
            var resetTemplate = _service.CreatePasswordResetTemplate(userEmail, userName, token, baseUrl);
            var companyTemplate = _service.CreateCompanyRegistrationTemplate(userEmail, userName, "Test Company", baseUrl);
            var userTemplate = _service.CreateUserCreatedTemplate(userEmail, userName, "temp123", baseUrl);

            // Assert
            Assert.All(new[] { validationTemplate, resetTemplate, companyTemplate, userTemplate }, template =>
            {
                Assert.True(template.IsHtml);
                Assert.Contains("DualComp CRM", template.Subject);
                Assert.Contains("DualComp CRM", template.Body);
                Assert.Contains("<!DOCTYPE html>", template.Body);
                Assert.Contains("</html>", template.Body);
            });
        }
    }
}
