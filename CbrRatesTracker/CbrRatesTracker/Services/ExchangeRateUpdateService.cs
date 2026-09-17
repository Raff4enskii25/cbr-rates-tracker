using CbrRatesTracker.CbrIntegration;
using CbrRatesTracker.Data;
using CbrRatesTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Services
{
    public class ExchangeRateUpdateService
    {
        private readonly CbrClientService _cbrClientService;
        private readonly AppDbContext _dbContext;
        private readonly ILogger<ExchangeRateUpdateService> _logger;

        public ExchangeRateUpdateService(CbrClientService cbrClientService, AppDbContext dbContext, ILogger<ExchangeRateUpdateService> logger)
        {
            _cbrClientService = cbrClientService;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task UpdateRatesAsync()
        {
            var latestRates = await _cbrClientService.GetLatestRatesAsync();
            var currencies = await _dbContext.Currencies.ToDictionaryAsync(c => c.CharCode, c => c);

            foreach (var latestRate in latestRates)
            {
                if (currencies.TryGetValue(latestRate.CharCode, out var currency))
                    AddExchangeRate(latestRate, currency);
                else
                {
                    var newCurrency = new Currency(latestRate.CharCode, latestRate.NumCode, latestRate.Name);
                    _dbContext.Currencies.Add(newCurrency);
                    _logger.LogInformation($"New currency added: {latestRate.CharCode} - {latestRate.Name}");
                    AddExchangeRate(latestRate, newCurrency);
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Exchange rates update completed on {DateTime.Now}.");
        }

        private void AddExchangeRate(ValuteDTO latestRate, Currency currency)
        {
            var newRate = new ExchangeRate(currency, latestRate.Date, latestRate.Nominal, latestRate.Value);
            _dbContext.Add(newRate);
            _logger.LogInformation($"New exchange rate added for {currency.CharCode} on {latestRate.Date}: {latestRate.Value}");
        }
    }
}
