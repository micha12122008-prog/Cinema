using Cinema.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinema.Infrastructure.BackgroundServices;

public class HealthCheckHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HealthCheckHostedService> _logger;
    private readonly string _logPath = "api_health_log.txt";
    private readonly DateTime _startTime;

    public HealthCheckHostedService(IServiceProvider serviceProvider, ILogger<HealthCheckHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _startTime = DateTime.UtcNow;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Health Check Service запущено.");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformHealthCheck();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Критична помилка під час виконання Health Check.");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task PerformHealthCheck()
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            bool isDbUp = false;
            string dbErrorMessage = "No connection";

            try
            {
                isDbUp = await context.Users.AnyAsync();
                dbErrorMessage = isDbUp ? "Connected" : "Table is empty";
            }
            catch (Exception ex)
            {
                isDbUp = false;
                dbErrorMessage = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError($"Деталі помилки БД: {dbErrorMessage}");
            }

            long memoryUsed = GC.GetTotalMemory(false) / 1024 / 1024;
            var uptime = DateTime.UtcNow - _startTime;

            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\n" +
                              $" - Статус БД: {(isDbUp ? "OK" : $"Error ({dbErrorMessage})")}\n" +
                              $" - Використано пам'яті: {memoryUsed} MB\n" +
                              $" - Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m\n";

            await File.AppendAllTextAsync(_logPath, logEntry);
        }
    }
}