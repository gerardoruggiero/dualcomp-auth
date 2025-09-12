using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dualcomp.Auth.Application.Services
{
    public class EmailValidationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EmailValidationBackgroundService> _logger;
        private readonly TimeSpan _period = TimeSpan.FromHours(24); // Ejecutar cada 24 horas

        public EmailValidationBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<EmailValidationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("EmailValidationBackgroundService iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var cleanupService = scope.ServiceProvider.GetRequiredService<EmailValidationCleanupService>();

                    _logger.LogInformation("Iniciando limpieza de tokens expirados y logs antiguos");

                    var result = await cleanupService.RunCleanupAsync(stoppingToken);

                    _logger.LogInformation(
                        "Limpieza completada: {ExpiredTokensDeleted} tokens expirados eliminados, {OldLogsDeleted} logs antiguos eliminados",
                        result.ExpiredTokensDeleted,
                        result.OldLogsDeleted);

                    await Task.Delay(_period, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("EmailValidationBackgroundService cancelado");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error durante la limpieza automática de tokens y logs");
                    
                    // Esperar 1 hora antes de reintentar en caso de error
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }

            _logger.LogInformation("EmailValidationBackgroundService detenido");
        }
    }
}
