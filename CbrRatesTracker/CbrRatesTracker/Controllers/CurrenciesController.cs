using CbrRatesTracker.Data;
using CbrRatesTracker.DTO;
using CbrRatesTracker.Models;
using CbrRatesTracker.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CbrRatesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly ExchangeRateQueryService _exchangeRateQueryService;

        public CurrenciesController(AppDbContext dbContext, ExchangeRateQueryService exchangeRateQueryService)
        {
            _dbContext = dbContext;
            _exchangeRateQueryService = exchangeRateQueryService;
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
        public async Task<ActionResult<IEnumerable<ExchangeRateLatestDTO>>> GetLatestRates()
        {
            var maxDate = await _dbContext.ExchangeRates.MaxAsync(e => (DateOnly?)e.Date);
            if (maxDate == null)
            {
                return NotFound("No exchange rates found.");
            }

            var exchangeRates = await _dbContext.ExchangeRates
                .Where(e => e.Date == maxDate)
                .Select(e => new ExchangeRateLatestDTO(
                    new ExchangeRateDTO(
                        e.Date,
                        e.Nominal,
                        e.Value
                    ),
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
            var result = await _exchangeRateQueryService.GetValidatedDataAsync(code, fromString, toString);
            if (!result.IsSuccess)
            {
                return MapError(result.Error!.Value);
            }

            var rateHistoryDTO = new ExchangeRateHistoryDTO(code, result.From, result.To, result.Rates);
            return Ok(rateHistoryDTO);
        }


        [HttpGet("{code}/stats")]
        public async Task<ActionResult<ExchangeRateStatsDTO>> GetCurrencyStats(string code, [FromQuery(Name = "from")] string fromString, [FromQuery(Name = "to")] string toString)
        {
            var result = await _exchangeRateQueryService.GetValidatedDataAsync(code, fromString, toString);
            if (!result.IsSuccess)
            {
                return MapError(result.Error!.Value);
            }

            var max = result.Rates.Max(e => e.Value / e.Nominal);
            var min = result.Rates.Min(e => e.Value / e.Nominal);
            var average = result.Rates.Average(e => e.Value / e.Nominal);

            var rateStatsDTO = new ExchangeRateStatsDTO(result.From, result.To, max, min, average);
            return Ok(rateStatsDTO);
        }


        private ActionResult MapError(RateQueryError error)
        {
            switch (error)
            {
                case RateQueryError.InvalidDateFormat:
                    return BadRequest("Invalid date format. Please use dd.MM.yyyy.");

                case RateQueryError.InvalidDateRange:
                    return BadRequest("'from' date must be less than or equal to 'to' date.");

                case RateQueryError.CurrencyNotFound:
                    return NotFound("Currency not found.");

                case RateQueryError.NoRatesFound:
                    return NotFound("No exchange rates found.");

                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error while processing the request.");
            }
        }
    }
}
