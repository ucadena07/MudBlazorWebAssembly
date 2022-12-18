
using MudTemplate.Client.Services.IServices;
using MudTemplate.Shared.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using MudTemplate.Shared.Utilities;

namespace MudTemplate.Client.Services.Http
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions defaultJsonSerializerOptions => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            HttpResponseMessage apiResponse = null;
            var URL = SD.BaseUrl+apiRequest.Url;
            switch (apiRequest.ApiType)
            {
                case SD.ApiType.GET:
                    apiResponse = await _httpClient.GetAsync(URL);
                    break;
                case SD.ApiType.POST:
                    var dataJson = System.Text.Json.JsonSerializer.Serialize(apiRequest.Data);
                    var stringContent = new StringContent(dataJson, Encoding.UTF8, "application/json");
                    apiResponse = await _httpClient.PostAsync(URL, stringContent);
                    break;
                default:
                    break;
            }
 
            try
            {    
                var response = await Deserialize<T>(apiResponse, defaultJsonSerializerOptions);
                return response;
       
            }
            catch (Exception e)
            {
                var temp = new APIResponse<T>();
                temp.ErrorMessages.Add(await apiResponse.Content.ReadAsStringAsync());
                temp.ErrorMessages.Add(e.StackTrace);
                temp.StatusCode = apiResponse.StatusCode;

                var APIResponse = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(temp));

                return APIResponse;
            }

        }

        private async Task<T> Deserialize<T>(HttpResponseMessage httpResponse, JsonSerializerOptions options)
        {
            var responseString = await httpResponse.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<T>(responseString, options);
        }

    }
}
