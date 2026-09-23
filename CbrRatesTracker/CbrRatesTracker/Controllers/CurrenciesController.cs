using CbrRatesTracker.Data;
using CbrRatesTracker.DTO;
using CbrRatesTracker.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CbrRatesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public CurrenciesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CurrencyDTO>>> GetCurrencies()
        {
            var currencies = await _dbContext.Currencies
                .Select(c => new CurrencyDTO(
                    c.CharCode,
                    c.NumCode,
                    c.Name
                    ))
                .ToListAsync();

            if (!currencies.Any())
            {
                return NotFound("No currencies found.");
            }

            return Ok(currencies);
        }


        [HttpGet("latest")]
        public async Task<ActionResult<IEnumerable<ExchangeRateDTO>>> GetLatestRates()
        {
            var maxDate = await _dbContext.ExchangeRates.MaxAsync(e => (DateOnly?)e.Date);
            if (maxDate == null)
            {
                return NotFound("No exchange rates found.");
            }

            var exchangeRates = await _dbContext.ExchangeRates
                .Where(e => e.Date == maxDate)
                .Select(e => new ExchangeRateDTO(
                    e.Date,
                    e.Nominal,
                    e.Value,
                    new CurrencyDTO(
                        e.Currency.CharCode,
                        e.Currency.NumCode,
                        e.Currency.Name
                    )
                ))
                .AsNoTracking()
                .ToListAsync();

            if (!exchangeRates.Any())
            {
                return NotFound("No exchange rates found.");
            }

            return Ok(exchangeRates);
        }


        [HttpGet("{code}/history")]
        public async Task<ActionResult<ExchangeRateHistoryDTO>> GetCurrencyHistory(string code, [FromQuery(Name = "from")] string fromString, [FromQuery(Name = "to")] string toString)
        {
            //TODO

            //if (!DateOnly.TryParse(fromString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly from))
            //    return BadRequest("Invalid 'from' date format. Please use 'dd.MM.yyyy'.");
            //if (!DateOnly.TryParse(toString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly to))
            //    return BadRequest("Invalid 'to' date format. Please use 'dd.MM.yyyy'.");

            //var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.CharCode == code);
            //if (!currencyExists)
            //    return NotFound($"Currency with code '{code}' not found.");

            //if (from > to)
            //    return BadRequest("Incorrect date range.");

            //var minDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MinAsync(e => (DateOnly?)e.Date);
            //var maxDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MaxAsync(e => (DateOnly?)e.Date);

            //if (minDate == null || maxDate == null)
            //    return NotFound("No exchange rates found.");

            //if (from < minDate)
            //    from = (DateOnly)minDate;
            //if (to > maxDate)
            //    to = (DateOnly)maxDate;

            //var rates = await _dbContext.ExchangeRates
            //    .Where(e => e.Currency.CharCode == code && e.Date >= from && e.Date <= to)
            //    .Select(e => new ExchangeRateDTO(
            //        e.Date,
            //        e.Nominal,
            //        e.Value,
            //        new CurrencyDTO(
            //            e.Currency.CharCode,
            //            e.Currency.NumCode,
            //            e.Currency.Name
            //        )
            //    ))
            //    .AsNoTracking()
            //    .ToListAsync();

            //if (!rates.Any())
            //    return NotFound("No exchange rates found.");

            var rateHistoryDTO = new ExchangeRateHistoryDTO(code, from, to, rates);

            return Ok(rateHistoryDTO);
        }


        [HttpGet("{code}/stats")]
        public async Task<ActionResult<ExchangeRateStatsDTO>> GetCurrencyStats(string code, [FromQuery(Name = "from")] string fromString, [FromQuery(Name = "to")] string toString)
        {
            //TODO

            //if (!DateOnly.TryParse(fromString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly from))
            //    return BadRequest("Invalid 'from' date format. Please use 'dd.MM.yyyy'.");
            //if (!DateOnly.TryParse(toString, CultureInfo.GetCultureInfo("ru-RU"), out DateOnly to))
            //    return BadRequest("Invalid 'to' date format. Please use 'dd.MM.yyyy'.");

            //var currencyExists = await _dbContext.Currencies.AnyAsync(c => c.CharCode == code);
            //if (!currencyExists)
            //    return NotFound($"Currency with code '{code}' not found.");

            //if (from > to)
            //    return BadRequest("Incorrect date range.");

            //var minDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MinAsync(e => (DateOnly?)e.Date);
            //var maxDate = await _dbContext.ExchangeRates.Where(e => e.Currency.CharCode == code).MaxAsync(e => (DateOnly?)e.Date);

            //if (minDate == null || maxDate == null)
            //    return NotFound("No exchange rates found.");

            //if (from < minDate)
            //    from = (DateOnly)minDate;
            //if (to > maxDate)
            //    to = (DateOnly)maxDate;

            //var rates = await _dbContext.ExchangeRates
            //    .Where(e => e.Currency.CharCode == code && e.Date >= from && e.Date <= to)
            //    .Select(e => new ExchangeRateDTO(
            //        e.Date,
            //        e.Nominal,
            //        e.Value,
            //        new CurrencyDTO(
            //            e.Currency.CharCode,
            //            e.Currency.NumCode,
            //            e.Currency.Name
            //        )
            //    ))
            //    .AsNoTracking()
            //    .ToListAsync();

            //if (!rates.Any())
            //    return NotFound("No exchange rates found.");

            var max = rates.Max(e => e.Value / e.Nominal);
            var min = rates.Min(e => e.Value / e.Nominal);
            var average = rates.Average(e => e.Value / e.Nominal);

            return new ExchangeRateStatsDTO(from, to, max, min, average);
        }
    }
}
