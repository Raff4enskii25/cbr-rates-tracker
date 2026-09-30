using CbrRatesTracker.Data;
using CbrRatesTracker.DTO;
using CbrRatesTracker.Enums;
using CbrRatesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Services
{
    public class AlertCreateService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AlertCreateService> _logger;

        public AlertCreateService(AppDbContext dbContext, ILogger<AlertCreateService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<AlertDTO?> CreateAlertAsync(CreateAlertDTO alertDTO)
        {
            var currency = await _dbContext.Currencies.Where(c => c.CharCode == alertDTO.CharCode).FirstOrDefaultAsync();
            if(currency == null)
            {
                _logger.LogInformation("Currency doesnt exist.");
                return null;
            }
            var alert = new Alert(currency, alertDTO.ThresholdValue, alertDTO.Direction, alertDTO.Email);
            _dbContext.Alerts.Add(alert);
            await _dbContext.SaveChangesAsync();
            
            _logger.LogInformation($"New alert for {alert.Currency.CharCode} has been created at {alert.CreatedAt}.");
            return new AlertDTO(alertDTO.CharCode, alert.ThresholdValue, alert.Direction, alert.IsTriggered, alert.CreatedAt, alert.TriggeredAt);
        }
    }
}
