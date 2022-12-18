using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Models;
using System.Net;

namespace MudTemplate.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly ILogging _customeLogger;
        private readonly IAccountRepository _accountRepository;
        private GeneralAPIResponse _response { get; set; }

        public AccountsController(ILogger<AccountsController> logger, ILogging customLogger, IAccountRepository accountRepository)
        {
            _logger = logger;
            _customeLogger = customLogger;
            _accountRepository = accountRepository;
            _response = new GeneralAPIResponse();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GeneralAPIResponse>> CreateUser([FromBody] User model)
        {

            await _accountRepository.CreateUser(model);
            _response.IsSuccess = true;
            _response.Result = true;
            _response.StatusCode = System.Net.HttpStatusCode.Created;
            return _response;

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<GeneralAPIResponse>> Login([FromBody] User model)
        {

            //Checks that password and username are provided
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Username and password must be provided");
                return BadRequest(_response);
            }

            //validate that user exists, is active, not blocked and password is correct
            var userValid = _accountRepository.VerifyUser(model);

            if (!userValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.ErrorMessages.Add("Invalid Credentials");
                return Unauthorized(_response);
            }

            _response.IsSuccess = true;
            _response.Result = true;
            _response.StatusCode = System.Net.HttpStatusCode.Created;
            return Ok(_response);


        }
    }
}
