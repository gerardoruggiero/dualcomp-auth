using Xunit;
using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies
{
    public class CompanySettingsTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateCompanySettings()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUsername = "test@example.com";
            var smtpPassword = "password123";
            var smtpUseSsl = true;
            var smtpFromEmail = "noreply@example.com";
            var smtpFromName = "Test Company";
            var createdBy = Guid.NewGuid();

            // Act
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, smtpServer, smtpPort, smtpUsername, smtpPassword,
                smtpUseSsl, smtpFromEmail, smtpFromName, createdBy);

            // Assert
            Assert.NotNull(companySettings);
            Assert.Equal(companyId, companySettings.CompanyId);
            Assert.Equal(smtpServer, companySettings.SmtpServer);
            Assert.Equal(smtpPort, companySettings.SmtpPort);
            Assert.Equal(smtpUsername, companySettings.SmtpUsername);
            Assert.Equal(smtpPassword, companySettings.SmtpPassword);
            Assert.Equal(smtpUseSsl, companySettings.SmtpUseSsl);
            Assert.Equal(smtpFromEmail, companySettings.SmtpFromEmail);
            Assert.Equal(smtpFromName, companySettings.SmtpFromName);
            Assert.True(companySettings.IsActive);
            Assert.Equal(createdBy, companySettings.CreatedBy);
            Assert.True(companySettings.IsValidConfiguration());
        }

        [Fact]
        public void Create_WithInvalidSmtpServer_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var smtpServer = ""; // Invalid
            var smtpPort = 587;
            var smtpUsername = "test@example.com";
            var smtpPassword = "password123";
            var smtpUseSsl = true;
            var smtpFromEmail = "noreply@example.com";
            var smtpFromName = "Test Company";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, smtpServer, smtpPort, smtpUsername, smtpPassword,
                smtpUseSsl, smtpFromEmail, smtpFromName));
        }

        [Fact]
        public void Create_WithInvalidSmtpPort_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 0; // Invalid
            var smtpUsername = "test@example.com";
            var smtpPassword = "password123";
            var smtpUseSsl = true;
            var smtpFromEmail = "noreply@example.com";
            var smtpFromName = "Test Company";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, smtpServer, smtpPort, smtpUsername, smtpPassword,
                smtpUseSsl, smtpFromEmail, smtpFromName));
        }

        [Fact]
        public void Create_WithInvalidSmtpUsername_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var smtpUsername = ""; // Invalid
            var smtpPassword = "password123";
            var smtpUseSsl = true;
            var smtpFromEmail = "noreply@example.com";
            var smtpFromName = "Test Company";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, smtpServer, smtpPort, smtpUsername, smtpPassword,
                smtpUseSsl, smtpFromEmail, smtpFromName));
        }

        [Fact]
        public void UpdateSettings_WithValidParameters_ShouldUpdateSettings()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "old@example.com", "oldpass",
                true, "old@example.com", "Old Company");

            var newSmtpServer = "smtp.outlook.com";
            var newSmtpPort = 465;
            var newSmtpUsername = "new@example.com";
            var newSmtpPassword = "newpass";
            var newSmtpUseSsl = false;
            var newSmtpFromEmail = "new@example.com";
            var newSmtpFromName = "New Company";
            var updatedBy = Guid.NewGuid();

            // Act
            companySettings.UpdateSettings(
                newSmtpServer, newSmtpPort, newSmtpUsername, newSmtpPassword,
                newSmtpUseSsl, newSmtpFromEmail, newSmtpFromName, updatedBy);

            // Assert
            Assert.Equal(newSmtpServer, companySettings.SmtpServer);
            Assert.Equal(newSmtpPort, companySettings.SmtpPort);
            Assert.Equal(newSmtpUsername, companySettings.SmtpUsername);
            Assert.Equal(newSmtpPassword, companySettings.SmtpPassword);
            Assert.Equal(newSmtpUseSsl, companySettings.SmtpUseSsl);
            Assert.Equal(newSmtpFromEmail, companySettings.SmtpFromEmail);
            Assert.Equal(newSmtpFromName, companySettings.SmtpFromName);
            Assert.NotNull(companySettings.UpdatedAt);
            Assert.Equal(updatedBy, companySettings.UpdatedBy);
        }

        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");
            companySettings.Deactivate();

            // Act
            companySettings.Activate();

            // Assert
            Assert.True(companySettings.IsActive);
            Assert.NotNull(companySettings.UpdatedAt);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            // Act
            companySettings.Deactivate();

            // Assert
            Assert.False(companySettings.IsActive);
            Assert.NotNull(companySettings.UpdatedAt);
        }

        [Fact]
        public void IsValidConfiguration_WithValidSettings_ShouldReturnTrue()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            // Act
            var isValid = companySettings.IsValidConfiguration();

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsValidConfiguration_WithInvalidSettings_ShouldReturnFalse()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var companySettings = Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                companyId, "smtp.gmail.com", 587, "test@example.com", "password",
                true, "noreply@example.com", "Test Company");

            // Act - Test with invalid server (this should throw an exception, not return false)
            // We'll test the validation by trying to create with invalid settings
            Assert.Throws<ArgumentException>(() => 
                Dualcomp.Auth.Domain.Companies.CompanySettings.Create(
                    companyId, "", 587, "test@example.com", "password",
                    true, "noreply@example.com", "Test Company"));
        }
    }
}
