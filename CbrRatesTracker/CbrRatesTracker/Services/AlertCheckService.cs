using CbrRatesTracker.Data;
using CbrRatesTracker.DTO;
using CbrRatesTracker.Enums;
using CbrRatesTracker.Interfaces;
using CbrRatesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Services
{
    public class AlertCheckService
    {
        private readonly AppDbContext _dbContext;
        private readonly INotificationService _notificationService;
        private readonly ILogger<AlertCheckService> _logger;

        public AlertCheckService(AppDbContext dbContext, INotificationService notificationService, ILogger<AlertCheckService> logger)
        {
            _dbContext = dbContext;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task CheckAlertsIsTriggeredAsync(IEnumerable<ExchangeRateLatestDTO> rates)
        {
            var alerts = await _dbContext.Alerts.Where(a => !a.IsTriggered).ToListAsync();

            if (!alerts.Any())
            {
                _logger.LogInformation("No alerts found.");
                return;
            }

            var ratesDict = rates.ToDictionary(r => r.Currency.CharCode);
            var triggeredAlerts = new List<Alert>();

            foreach (var alert in alerts)
            {
                if (!ratesDict.TryGetValue(alert.Currency.CharCode, out var rateDto))
                    continue;

                decimal rate = rateDto.Rate.Value / rateDto.Rate.Nominal;

                switch (alert.Direction)
                {
                    case Direction.Above:
                        if (alert.ThresholdValue < rate)
                            alert.SetAlertIsTriggered(DateOnly.FromDateTime(DateTime.Now));
                        break;

                    case Direction.Below:
                        if (rate < alert.ThresholdValue)
                            alert.SetAlertIsTriggered(DateOnly.FromDateTime(DateTime.Now));
                        break;
                }

                if (alert.IsTriggered)
                {
                    triggeredAlerts.Add(alert);
                    _logger.LogInformation($"Alert triggered for {alert.Currency.Name} at {alert.TriggeredAt}");

                    if (alert.Email != null)
                    {
                        try
                        {
                            await _notificationService.SendMessageAsync(
                                alert.Email,
                                $"Alert for {alert.Currency.Name} triggered at {alert.TriggeredAt}.\n",
                                $"Dear user, your alert for {alert.Currency.Name} triggered at {alert.TriggeredAt}.\n" +
                                $"Current exchange rate for {alert.Currency.Name} is {rate}.\n" +
                                $"Threshold value for alert was {alert.ThresholdValue}."
                            );
                            _logger.LogInformation($"The message has sended to {alert.Email} at {DateTime.Now}");
                        }
                        catch(Exception ex)
                        {
                            _logger.LogError(ex, "An error occured while sending the message.");
                        }
                    }
                }
            }

            if (triggeredAlerts.Any())
            {
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
