﻿using MudTemplate.Client.Services.IServices;
using MudTemplate.Shared.Models;
using MudTemplate.Shared.Utilities;

namespace MudTemplate.Client.Services
{
    public class AccountService : IAccountService
    {
        private readonly IHttpService _httpService;

        public AccountService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<T> CreateUser<T>(User setObj)
        {
            return await _httpService.SendAsync<T>(new APIRequest
            {
                ApiType = SD.ApiType.POST,
                Url = "api/Accounts/CreateUser",
                Data = setObj
            });

        }
    }
}
