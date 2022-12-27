using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MudTemplate.Backend.Helpers.Interfaces;
using MudTemplate.Shared.Entities;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Models;
using MudTemplate.Shared.Utilities;
using MudTemplate.Shared.Utilities.IUtilities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MudTemplate.Backend.Services
{
    public class JwtService : IJwtService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordService _passwordService;
        private readonly IGlobalSettings _config;
        public JwtService(ApplicationDbContext context, IPasswordService passwordService, IGlobalSettings config)
        {
            _context = context;
            _passwordService = passwordService;
            _config = config;
        }

        public async Task<AuthResponse> GetTokenAsync(AuthRequest authRequest, string ipAddress)
        {

            var user = await _context.SiteUsers.FirstOrDefaultAsync(it => it.UserName.Equals(authRequest.UserName));
            if (user == null)
            {
                return await Task.FromResult<AuthResponse>(null);
            }

            var verifyPassword = _passwordService.VerifyPasswordHash(authRequest.Password, user.PasswordHash, user.PasswordSalt);

            if (!verifyPassword)
            {
                return await Task.FromResult<AuthResponse>(null);
            }

            var stringToken = await GenerateToken(user.UserName);
            var refreshToken = GenerateRefreshToken();



            return await SaveTokenDetails(ipAddress, user.UserId, stringToken, refreshToken);
        }

        private async Task<AuthResponse> SaveTokenDetails(string ipAddress, int userId, string tokenString, string refreshToken)
        {
            var userRefreshToken = new RefreshToken
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = DateTime.UtcNow.AddMinutes(5),
                IpAddress = ipAddress,
                IsInvalidated = false,
                UserRefreshToken = refreshToken,
                Token = tokenString,
                UserId = userId
            };

            await _context.RefreshTokens.AddAsync(userRefreshToken);
            await _context.SaveChangesAsync();


            var response = new AuthResponse
            {
                Token = tokenString,
                RefreshToken = refreshToken,
                IsSuccess = true,
                TokenExpDate = userRefreshToken.ExpirationDate

            };

            return await Task.FromResult(response);
        }


        public async Task<AuthResponse> GetRefreshTokenAsync(string ipAddress, int userId, string userName)
        {
            var refreshToken = GenerateRefreshToken();
            var accessToken = await GenerateToken(userName);
            return await SaveTokenDetails(ipAddress, userId, accessToken, refreshToken);
        }

        private string GenerateRefreshToken()
        {
            var byteArray = new byte[64];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {

                randomNumberGenerator.GetBytes(byteArray);
                return Convert.ToBase64String(byteArray);
            }
        }

        private async Task<string> GenerateToken(string userName)
        {
            //get jwt and application keys
            var jwtKey = await _config.GetJwtKey();
            var applicationId = await _config.GetApplicationId();
            var masterKey = $"{jwtKey}:{applicationId}";
            var keyBytes = Encoding.UTF8.GetBytes(masterKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, userName),
            };

            JwtSecurityToken token = new(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(5),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256)
            );

            return tokenHandler.WriteToken(token);
        }




        public async Task<bool> IsTokenValid(string accessToken, string ipAddress)
        {
            var isValid = _context.RefreshTokens.FirstOrDefault(it => it.Token == accessToken && it.IpAddress == ipAddress) is not null;
            return await Task.FromResult(isValid);
        }


    }
}
