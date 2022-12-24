using MudTemplate.Backend.Helpers.Interfaces;

namespace MudTemplate.Server.Helpers
{
    public class GlobalSettings : IGlobalSettings
    {
        private readonly IConfiguration _config;
        public GlobalSettings(IConfiguration config)
        {
            _config= config;
        }
        public async Task<string> GetApplicationId()
        {
            return await Task.FromResult(_config.GetValue<string>("Application:ApplicationId"));
         
        }

        public async Task<string> GetCurrentUserId()
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetJwtKey()
        {
            return await Task.FromResult(_config.GetValue<string>("JwtSettings:Key"));
        }
    }
}
