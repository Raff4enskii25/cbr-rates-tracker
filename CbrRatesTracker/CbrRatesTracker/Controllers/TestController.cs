using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CbrRatesTracker.CbrIntegration;

namespace CbrRatesTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        public CbrClientService _cbrService;

        public TestController(CbrClientService cbrService)
        {
            _cbrService = cbrService;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            await _cbrService.GetLatestRatesAsync();
            return Ok("Test endpoint is working.");
        }
    }
}
