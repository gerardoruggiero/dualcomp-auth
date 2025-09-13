using DualComp.Infraestructure.Mail.Interfaces;
using DualComp.Infraestructure.Mail.Models;
using DualComp.Infraestructure.Mail.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace DualComp.Infraestructure.Mail.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMailServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configurar SMTP por defecto
            services.Configure<SmtpConfiguration>(configuration.GetSection("Smtp"));
            
            // Registrar servicios
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            
            // Configurar SMTP por defecto desde configuración
            services.AddSingleton(provider =>
            {
                var config = configuration.GetSection("Smtp").Get<SmtpConfiguration>();
                return config ?? new SmtpConfiguration
                {
                    Server = "smtp.gmail.com",
                    Port = 587,
                    Username = "noreply@dualcomp.com",
                    Password = "ENCRYPTED_PASSWORD_HERE",
                    UseSsl = true,
                    FromEmail = "noreply@dualcomp.com",
                    FromName = "DualComp CRM",
                    Timeout = 30000
                };
            });

            return services;
        }

        public static IServiceCollection AddMailServices(this IServiceCollection services, SmtpConfiguration smtpConfiguration)
        {
            // Registrar servicios
            services.AddScoped<IEmailService, SmtpEmailService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            
            // Configurar SMTP específico
            services.AddSingleton(smtpConfiguration);

            return services;
        }
    }
}

