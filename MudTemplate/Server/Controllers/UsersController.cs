using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudTemplate.Backend.Repositories;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Models;

namespace MudTemplate.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ILogging _customeLogger;
        private readonly IUserRepository _userRepository;
        private GeneralAPIResponse _response { get; set; }

        public UsersController(ILogger<UsersController> logger, ILogging customLogger, IUserRepository userRepository)
        {
            _logger = logger;
            _customeLogger = customLogger;
            _userRepository = userRepository;
            _response = new GeneralAPIResponse();
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GeneralAPIResponse>> Get(int id)
        {
            var user = await _userRepository.Get(id);

            if(user is null)
            {
                _response.IsSuccess = true;
                _response.Result = null;
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                return NotFound(_response);
            }

            _response.IsSuccess = true;
            _response.Result = user;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GeneralAPIResponse>> GetAll()
        {
            _response.IsSuccess = true;
            _response.Result = await _userRepository.GetAll();
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}