using Microsoft.AspNetCore.Mvc;
using MudTemplate.Server.Helpers.Interfaces;

namespace MudTemplate.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILogging _customeLogger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILogging customLogger)
        {
            _logger = logger;
            _customeLogger = customLogger;  
        }

        [HttpGet]
        public List<string> TestEndpoint()
        {
            try
            {
                _customeLogger.Log("Getting villas", "normal");
                return new List<string>() { "test", "test2" };
            }
            catch (Exception)
            {

                throw;
            }
      
        }

    }
}