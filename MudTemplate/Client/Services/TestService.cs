using MudTemplate.Backend.Utilities;
using MudTemplate.Client.Services.IServices;
using MudTemplate.Shared.Models;

namespace MudTemplate.Client.Services
{
    public class TestService : ITestService
    {
        private readonly IHttpService _httpService;

        public TestService(IHttpService httpService)
        {
            _httpService = httpService;
        }
        public async Task<T> GetTest<T>()
        {
          return await _httpService.SendAsync<T>(new APIRequest
            {
                ApiType = SD.ApiType.GET,
                Url = "WeatherForecast/Get",
            });


        }
    }
}
