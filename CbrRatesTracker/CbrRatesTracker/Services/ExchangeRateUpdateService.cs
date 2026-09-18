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
            if(latestRates.Count == 0)
            {
                _logger.LogWarning("No rates received from CBR. Skipping update.");
                return;
            }

            var currencies = await _dbContext.Currencies.ToDictionaryAsync(c => c.CharCode, c => c);
            var existingRates = _dbContext.ExchangeRates
                .Where(r => r.Date == latestRates[0].Date)
                .Select(r => r.Currency.CharCode)
                .ToHashSet();

            foreach (var latestRate in latestRates)
            {
                if (currencies.TryGetValue(latestRate.CharCode, out var currency))
                    AddExchangeRate(latestRate, currency, existingRates);
                else
                {
                    var newCurrency = new Currency(latestRate.CharCode, latestRate.NumCode, latestRate.Name);
                    _dbContext.Currencies.Add(newCurrency);
                    _logger.LogInformation($"New currency added: {latestRate.CharCode} - {latestRate.Name}");
                    AddExchangeRate(latestRate, newCurrency, existingRates);
                }
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Exchange rates update completed on {DateTime.Now}.");
        }


        private void AddExchangeRate(ValuteDTO latestRate, Currency currency, HashSet<string> existingRates)
        {   
            if(existingRates.Contains(currency.CharCode))
            {
                _logger.LogInformation($"Exchange rate for {currency.CharCode} on {latestRate.Date} already exists. Skipping.");
                return;
            }
            
            var newRate = new ExchangeRate(currency, latestRate.Date, latestRate.Nominal, latestRate.Value);
            _dbContext.Add(newRate);
            _logger.LogInformation($"New exchange rate added for {currency.CharCode} on {latestRate.Date}: {latestRate.Value}");
        }
    }
}
