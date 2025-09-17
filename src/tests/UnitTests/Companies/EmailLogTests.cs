using Dualcomp.Auth.Domain.Companies;

namespace Dualcomp.Auth.UnitTests.Companies
{
    public class EmailLogTests
    {
        [Fact]
        public void Create_WithValidParameters_ShouldCreateEmailLog()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "test@example.com";
            var subject = "Test Subject";
            var emailType = "Validation";
            var status = "Pending";

            // Act
            var emailLog = EmailLog.Create(companyId, toEmail, subject, emailType, status);

            // Assert
            Assert.NotNull(emailLog);
            Assert.Equal(companyId, emailLog.CompanyId);
            Assert.Equal(toEmail, emailLog.ToEmail);
            Assert.Equal(subject, emailLog.Subject);
            Assert.Equal(emailType, emailLog.EmailType);
            Assert.Equal(status, emailLog.Status);
            Assert.Null(emailLog.ErrorMessage);
            Assert.Null(emailLog.SentAt);
            Assert.True(emailLog.IsPending);
            Assert.False(emailLog.IsSent);
            Assert.False(emailLog.IsFailed);
        }

        [Fact]
        public void Create_WithErrorMessage_ShouldCreateEmailLogWithError()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "test@example.com";
            var subject = "Test Subject";
            var emailType = "Validation";
            var status = "Failed";
            var errorMessage = "SMTP connection failed";

            // Act
            var emailLog = EmailLog.Create(companyId, toEmail, subject, emailType, status, errorMessage);

            // Assert
            Assert.NotNull(emailLog);
            Assert.Equal(errorMessage, emailLog.ErrorMessage);
            Assert.True(emailLog.IsFailed);
            Assert.False(emailLog.IsSent);
            Assert.False(emailLog.IsPending);
        }

        [Fact]
        public void Create_WithEmptyToEmail_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "";
            var subject = "Test Subject";
            var emailType = "Validation";
            var status = "Pending";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailLog.Create(companyId, toEmail, subject, emailType, status));
        }

        [Fact]
        public void Create_WithEmptySubject_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "test@example.com";
            var subject = "";
            var emailType = "Validation";
            var status = "Pending";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailLog.Create(companyId, toEmail, subject, emailType, status));
        }

        [Fact]
        public void Create_WithEmptyEmailType_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "test@example.com";
            var subject = "Test Subject";
            var emailType = "";
            var status = "Pending";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailLog.Create(companyId, toEmail, subject, emailType, status));
        }

        [Fact]
        public void Create_WithEmptyStatus_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var toEmail = "test@example.com";
            var subject = "Test Subject";
            var emailType = "Validation";
            var status = "";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => EmailLog.Create(companyId, toEmail, subject, emailType, status));
        }

        [Fact]
        public void MarkAsSent_WhenPending_ShouldMarkAsSent()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Pending");

            // Act
            emailLog.MarkAsSent();

            // Assert
            Assert.Equal("Sent", emailLog.Status);
            Assert.NotNull(emailLog.SentAt);
            Assert.True(emailLog.IsSent);
            Assert.False(emailLog.IsPending);
            Assert.False(emailLog.IsFailed);
            Assert.Null(emailLog.ErrorMessage);
        }

        [Fact]
        public void MarkAsSent_WhenAlreadySent_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Pending");
            emailLog.MarkAsSent();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => emailLog.MarkAsSent());
        }

        [Fact]
        public void MarkAsFailed_WhenPending_ShouldMarkAsFailed()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Pending");
            var errorMessage = "SMTP connection failed";

            // Act
            emailLog.MarkAsFailed(errorMessage);

            // Assert
            Assert.Equal("Failed", emailLog.Status);
            Assert.Equal(errorMessage, emailLog.ErrorMessage);
            Assert.True(emailLog.IsFailed);
            Assert.False(emailLog.IsSent);
            Assert.False(emailLog.IsPending);
        }

        [Fact]
        public void MarkAsFailed_WhenAlreadySent_ShouldThrowException()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Pending");
            emailLog.MarkAsSent();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => emailLog.MarkAsFailed("Error"));
        }

        [Fact]
        public void MarkAsFailed_WithEmptyErrorMessage_ShouldUseDefaultMessage()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Pending");

            // Act
            emailLog.MarkAsFailed("");

            // Assert
            Assert.Equal("Unknown error", emailLog.ErrorMessage);
        }

        [Fact]
        public void MarkAsPending_ShouldSetStatusToPending()
        {
            // Arrange
            var companyId = Guid.NewGuid();
            var emailLog = EmailLog.Create(companyId, "test@example.com", "Test Subject", "Validation", "Failed", "Error");
            emailLog.MarkAsFailed("Error");

            // Act
            emailLog.MarkAsPending();

            // Assert
            Assert.Equal("Pending", emailLog.Status);
            Assert.True(emailLog.IsPending);
            Assert.False(emailLog.IsSent);
            Assert.False(emailLog.IsFailed);
            Assert.Null(emailLog.ErrorMessage);
        }
    }
}
