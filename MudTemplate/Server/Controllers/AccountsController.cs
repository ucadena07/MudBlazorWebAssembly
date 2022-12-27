using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MudTemplate.Backend;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Shared.Entities;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MudTemplate.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly ILogger<AccountsController> _logger;
        private readonly ILogging _customeLogger;
        private readonly IAccountRepository _accountRepository;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private GeneralAPIResponse _response { get; set; }

        public AccountsController(ILogger<AccountsController> logger, ILogging customLogger, IAccountRepository accountRepository, IJwtService jwtService, ApplicationDbContext context)
        {
            _logger = logger;
            _customeLogger = customLogger;
            _accountRepository = accountRepository;
            _response = new GeneralAPIResponse();
            _jwtService = jwtService;
            _context = context;
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
        public async Task<ActionResult<GeneralAPIResponse>> Login([FromBody] AuthRequest request)
        {
            //Checks that password and username are provided
            if (!ModelState.IsValid)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.AddModelStateErrors(ModelState);
                return BadRequest(_response);
            }

            //validate that user exists, is active, not blocked and password is correct
            var authoResponse = await _jwtService.GetTokenAsync(request, HttpContext.Connection.RemoteIpAddress.ToString());

            if (authoResponse is null)
            {
                _response.IsSuccess = true;
                _response.Result = null;
                _response.StatusCode = HttpStatusCode.Unauthorized;
                _response.ErrorMessages.Add("Invalid Credentials");
                return Unauthorized(_response);
            }

            _response.IsSuccess = true;
            _response.Result = authoResponse;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return Ok(_response);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GeneralAPIResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add("Tokens must be provided");
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            string ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();

            var token = GetJwtToken(request.ExpiredToken);

            var userRefreshToken = _context.RefreshTokens.FirstOrDefault(it => it.IsInvalidated == false
                                    && it.Token == request.ExpiredToken &&
                                    it.UserRefreshToken == request.RefreshToken
                                    && it.IpAddress == ipAddress);

            AuthResponse authReponse = ValidateDetails(token, userRefreshToken);

            if (!authReponse.IsSuccess)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages.Add(authReponse.Reason.ToString());
                return BadRequest(_response);
            }


            userRefreshToken.IsInvalidated = true;
            _context.RefreshTokens.Update(userRefreshToken);
            await _context.SaveChangesAsync();

            var userName = token.Claims.FirstOrDefault(it => it.Type == ClaimTypes.Name).Value;
            var dbReponse = await _jwtService.GetRefreshTokenAsync(ipAddress, userRefreshToken.UserId, userName);
            _response.Result = dbReponse;

            return Ok(_response);
        }

        private AuthResponse ValidateDetails(JwtSecurityToken token, RefreshToken userRefreshToken)
        {
            if (userRefreshToken is null)
                return new AuthResponse { IsSuccess = false, Reason = "Invalid Token Details" };
            if (token.ValidTo.Subtract(DateTime.UtcNow) > TimeSpan.FromMinutes(2))
                return new AuthResponse { IsSuccess = false, Reason = "Token not expired" };
            if (!userRefreshToken.IsActive)
                return new AuthResponse { IsSuccess = false, Reason = "Refresh token expired" };
            return new AuthResponse { IsSuccess = true };

        }


        private JwtSecurityToken GetJwtToken(string expiredToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ReadJwtToken(expiredToken);
        }
    }
}
