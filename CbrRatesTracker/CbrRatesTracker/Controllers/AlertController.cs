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
    public class AlertsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly AlertCreateService _service;

        public AlertsController(AppDbContext dbContext, AlertCreateService service)
        {
            _dbContext = dbContext;
            _service = service;
        }


        [HttpGet]
        public async Task<IActionResult> GetAlerts([FromQuery(Name = "currency")] string currency, [FromQuery(Name = "isTriggered")] bool? isTriggered)
        {
            if(currency == null)
                return BadRequest("Specify the currency.");

            var query = _dbContext.Alerts.AsQueryable().Where(a => a.Currency.Name == currency || a.Currency.CharCode == currency);
            
            if (isTriggered.HasValue)
                query = query.Where(a => a.IsTriggered == isTriggered);

            var alerts = await query.Select(a => new AlertDTO(
                a.Currency.CharCode, 
                a.ThresholdValue, 
                a.Direction, 
                a.IsTriggered, 
                a.CreatedAt, 
                a.TriggeredAt)
            ).ToListAsync();
            
            if (!alerts.Any())
                return Ok("No alerts found.");

            return Ok(alerts);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAlert([FromBody] CreateAlertDTO alertDTO)
        {
            var alert = await _service.CreateAlertAsync(alertDTO);
            if (alert == null)
            {
                return BadRequest("Currency doesnt exist.");
            }
           
            return Ok(alert);
        }
    }
}
