using MudTemplate.Client.Services.IServices;
using MudTemplate.Shared.Models;
using MudTemplate.Shared.Utilities;

namespace MudTemplate.Client.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpService _httpService;

        public UserService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<T> Get<T>(int id)
        {
            return await _httpService.SendAsync<T>(new APIRequest
            {
                ApiType = SD.ApiType.GET,
                Url = $"api/Users/Get/{id}"
                
            });
        }

        public async Task<T> GetAll<T>()
        {
            return await _httpService.SendAsync<T>(new APIRequest
            {
                ApiType = SD.ApiType.GET,
                Url = "api/Users/GetAll",    
            });
        }
    }
}
