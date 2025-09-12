using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;

namespace DualComp.Infraestructure.Mail.Services
{
    public class EmailTemplateService : IEmailTemplateService
    {
        public EmailMessage CreateEmailValidationTemplate(string userEmail, string userName, string validationToken, string baseUrl)
        {
            var validationUrl = $"{baseUrl.TrimEnd('/')}/validate-email?token={validationToken}";
            
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Validación de Email - DualComp CRM</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #2c3e50; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f8f9fa; }}
        .button {{ display: inline-block; background-color: #3498db; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>DualComp CRM</h1>
        </div>
        <div class='content'>
            <h2>¡Bienvenido, {userName}!</h2>
            <p>Gracias por registrarte en DualComp CRM. Para completar tu registro, necesitas validar tu dirección de email.</p>
            <p>Haz clic en el siguiente botón para validar tu cuenta:</p>
            <p style='text-align: center;'>
                <a href='{validationUrl}' class='button'>Validar Mi Email</a>
            </p>
            <p>Si el botón no funciona, puedes copiar y pegar este enlace en tu navegador:</p>
            <p style='word-break: break-all; background-color: #e9ecef; padding: 10px; border-radius: 3px;'>
                {validationUrl}
            </p>
            <p><strong>Importante:</strong> Este enlace expirará en 24 horas por motivos de seguridad.</p>
        </div>
        <div class='footer'>
            <p>Este email fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
            <p>&copy; 2025 DualComp CRM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = userEmail,
                ToName = userName,
                Subject = "Validación de Email - DualComp CRM",
                Body = htmlBody,
                IsHtml = true
            };
        }

        public EmailMessage CreatePasswordResetTemplate(string userEmail, string userName, string resetToken, string baseUrl)
        {
            var resetUrl = $"{baseUrl.TrimEnd('/')}/reset-password?token={resetToken}";
            
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Restablecer Contraseña - DualComp CRM</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #e74c3c; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f8f9fa; }}
        .button {{ display: inline-block; background-color: #e74c3c; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>DualComp CRM</h1>
        </div>
        <div class='content'>
            <h2>Restablecer Contraseña</h2>
            <p>Hola {userName},</p>
            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta en DualComp CRM.</p>
            <p>Haz clic en el siguiente botón para crear una nueva contraseña:</p>
            <p style='text-align: center;'>
                <a href='{resetUrl}' class='button'>Restablecer Contraseña</a>
            </p>
            <p>Si el botón no funciona, puedes copiar y pegar este enlace en tu navegador:</p>
            <p style='word-break: break-all; background-color: #e9ecef; padding: 10px; border-radius: 3px;'>
                {resetUrl}
            </p>
            <div class='warning'>
                <p><strong>⚠️ Importante:</strong></p>
                <ul>
                    <li>Este enlace expirará en 24 horas por motivos de seguridad</li>
                    <li>Si no solicitaste este cambio, puedes ignorar este email</li>
                    <li>Tu contraseña actual seguirá siendo válida hasta que la cambies</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>Este email fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
            <p>&copy; 2025 DualComp CRM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = userEmail,
                ToName = userName,
                Subject = "Restablecer Contraseña - DualComp CRM",
                Body = htmlBody,
                IsHtml = true
            };
        }

        public EmailMessage CreateCompanyRegistrationTemplate(string userEmail, string userName, string companyName, string baseUrl)
        {
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Registro de Empresa - DualComp CRM</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #27ae60; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f8f9fa; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .success {{ background-color: #d4edda; border: 1px solid #c3e6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>DualComp CRM</h1>
        </div>
        <div class='content'>
            <h2>¡Empresa Registrada Exitosamente!</h2>
            <p>Hola {userName},</p>
            <div class='success'>
                <p><strong>✅ ¡Felicitaciones!</strong></p>
                <p>La empresa <strong>{companyName}</strong> ha sido registrada exitosamente en DualComp CRM.</p>
            </div>
            <p>Ahora puedes:</p>
            <ul>
                <li>Gestionar empleados y usuarios</li>
                <li>Configurar la información de tu empresa</li>
                <li>Personalizar la configuración de emails</li>
                <li>Acceder a todas las funcionalidades del CRM</li>
            </ul>
            <p>Si tienes alguna pregunta o necesitas ayuda, no dudes en contactarnos.</p>
        </div>
        <div class='footer'>
            <p>Este email fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
            <p>&copy; 2025 DualComp CRM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = userEmail,
                ToName = userName,
                Subject = $"Empresa {companyName} Registrada - DualComp CRM",
                Body = htmlBody,
                IsHtml = true
            };
        }

        public EmailMessage CreateUserCreatedTemplate(string userEmail, string userName, string temporaryPassword, string baseUrl)
        {
            var loginUrl = $"{baseUrl.TrimEnd('/')}/login";
            
            var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Nueva Cuenta Creada - DualComp CRM</title>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #8e44ad; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 30px; background-color: #f8f9fa; }}
        .button {{ display: inline-block; background-color: #8e44ad; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
        .credentials {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .warning {{ background-color: #f8d7da; border: 1px solid #f5c6cb; padding: 15px; border-radius: 5px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>DualComp CRM</h1>
        </div>
        <div class='content'>
            <h2>¡Nueva Cuenta Creada!</h2>
            <p>Hola {userName},</p>
            <p>Se ha creado una nueva cuenta para ti en DualComp CRM.</p>
            <div class='credentials'>
                <p><strong>📧 Email:</strong> {userEmail}</p>
                <p><strong>🔑 Contraseña Temporal:</strong> <code>{temporaryPassword}</code></p>
            </div>
            <p>Haz clic en el siguiente botón para iniciar sesión:</p>
            <p style='text-align: center;'>
                <a href='{loginUrl}' class='button'>Iniciar Sesión</a>
            </p>
            <div class='warning'>
                <p><strong>⚠️ Importante:</strong></p>
                <ul>
                    <li>Debes cambiar tu contraseña temporal en el primer inicio de sesión</li>
                    <li>Esta contraseña temporal es válida por tiempo limitado</li>
                    <li>No compartas tus credenciales con nadie</li>
                </ul>
            </div>
        </div>
        <div class='footer'>
            <p>Este email fue enviado automáticamente. Por favor, no respondas a este mensaje.</p>
            <p>&copy; 2025 DualComp CRM. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>";

            return new EmailMessage
            {
                To = userEmail,
                ToName = userName,
                Subject = "Nueva Cuenta Creada - DualComp CRM",
                Body = htmlBody,
                IsHtml = true
            };
        }
    }
}
