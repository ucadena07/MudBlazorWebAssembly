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
        public async Task<ActionResult<GeneralAPIResponse>> Get(int id)
        {
            try
            {
                _response.IsSuccess = true;
                _response.Result = await _userRepository.Get(id);
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages.Add(e.Message);
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<GeneralAPIResponse>> GetAll()
        {
            try
            {
                _response.IsSuccess = true;
                _response.Result = await _userRepository.GetAll();
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception e)
            {

                _response.IsSuccess = false;
                _response.ErrorMessages.Add(e.Message);
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
    }
}