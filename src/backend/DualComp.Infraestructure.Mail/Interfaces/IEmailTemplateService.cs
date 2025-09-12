using DualComp.Infraestructure.Mail.Models;

namespace DualComp.Infraestructure.Mail.Interfaces
{
    public interface IEmailTemplateService
    {
        EmailMessage CreateEmailValidationTemplate(string userEmail, string userName, string validationToken, string baseUrl);
        EmailMessage CreatePasswordResetTemplate(string userEmail, string userName, string resetToken, string baseUrl);
        EmailMessage CreateCompanyRegistrationTemplate(string userEmail, string userName, string companyName, string baseUrl);
        EmailMessage CreateUserCreatedTemplate(string userEmail, string userName, string temporaryPassword, string baseUrl);
    }
}
