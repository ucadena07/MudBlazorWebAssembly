using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Shared.Models;

namespace MudTemplate.Server.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ILogging _customeLogger;
        private APIResponseV2 _response { get; set; }

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ILogging customLogger)
        {
            _logger = logger;
            _customeLogger = customLogger;  
            _response= new APIResponseV2(); 
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponseV2>> Get()
        {
            try
            {
                _response.IsSuccess = true;
                _response.Result = new List<string>() { "test", "test2" };
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(e.Message);
                throw;
            }
      
        }

    }
}