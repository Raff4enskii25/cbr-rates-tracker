using Microsoft.EntityFrameworkCore;
using CbrRatesTracker.DTO;
using CbrRatesTracker.Models;
using System.Globalization;
using CbrRatesTracker.Data;

namespace CbrRatesTracker.Services
{
    public class ExchangeRateQueryService
    {
        private readonly AppDbContext _dbContext;

        public ExchangeRateQueryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RateQueryResult> GetValidatedDataAsync(string code, string fromString, string toString)
        {
            if (!DateOnly.TryParse(fromString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly from))
                return RateQueryResult.Fail(RateQueryError.InvalidDateFormat);
            
            if (!DateOnly.TryParse(toString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly to))
                return RateQueryResult.Fail(RateQueryError.InvalidDateFormat);

            var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.CharCode == code);
            if (!currencyExists)
                return RateQueryResult.Fail(RateQueryError.CurrencyNotFound);

            if (from > to)
                return RateQueryResult.Fail(RateQueryError.InvalidDateRange);

            var minDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MinAsync(e => (DateOnly?)e.Date);
            var maxDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MaxAsync(e => (DateOnly?)e.Date);

            if (minDate == null || maxDate == null)
                return RateQueryResult.Fail(RateQueryError.NoRatesFound);

            if (from < minDate)
                from = (DateOnly)minDate;
            if (to > maxDate)
                to = (DateOnly)maxDate;

            var rates = await _dbContext.ExchangeRates
                .Where(e => e.Currency.CharCode == code && e.Date >= from && e.Date <= to)
                .Select(e => new ExchangeRateDTO(
                    e.Date,
                    e.Nominal,
                    e.Value
                ))
                .AsNoTracking()
                .ToListAsync();

            if (!rates.Any())
                return RateQueryResult.Fail(RateQueryError.NoRatesFound);

            return RateQueryResult.Success(from, to, rates);
        }
    }
}
