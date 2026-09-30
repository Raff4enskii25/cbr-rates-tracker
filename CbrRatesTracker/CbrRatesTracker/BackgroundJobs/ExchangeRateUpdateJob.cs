using Microsoft.Extensions.Hosting;
using CbrRatesTracker.Services;

namespace CbrRatesTracker.BackgroundJobs
{
    public class ExchangeRateUpdateJob : BackgroundService
    {
        private readonly ILogger<ExchangeRateUpdateJob> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public ExchangeRateUpdateJob(ILogger<ExchangeRateUpdateJob> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExchangeRateUpdateJob is running.");
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromDays(1));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    _logger.LogInformation("ExchangeRateUpdateJob is executing at: {time}", DateTime.Now);
                    try
                    {
                        using (var scope = _scopeFactory.CreateScope())
                        {
                            var ratesUpdateService = scope.ServiceProvider.GetRequiredService<ExchangeRateUpdateService>();
                            var alertCheckService = scope.ServiceProvider.GetRequiredService<AlertCheckService>();
                            var rates = await ratesUpdateService.UpdateRatesAsync();
                            if (rates != null && rates.Any())
                            {
                                try
                                {
                                    await alertCheckService.CheckAlertsIsTriggeredAsync(rates);
                                }
                                catch(Exception ex)
                                {
                                    _logger.LogError(ex, "An error occured while checking alert triggers.");
                                }
                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"An error occurred while updating exchange rates.");
                    }
                }

                _logger.LogInformation("ExchangeRateUpdateJob has stopped.");
            }

            catch (OperationCanceledException)
            {
                _logger.LogInformation("ExchangeRateUpdateJob is stopping due to cancellation.");
            }
        }
    }
}
