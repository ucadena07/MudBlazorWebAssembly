

using MudTemplate.Shared.Models;
using Newtonsoft.Json;
using System.Text.Json;
using System.Text;
using MudTemplate.Shared.Utilities;
using MudTemplate.Shared.IServices;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using MudTemplate.Components.Helpers.JsExtensions;
using Microsoft.Identity.Client;
using MudBlazor;

namespace MudTemplate.Client.Services.Http
{
    public class HttpService : IHttpService
    {
        private readonly string TOKENKEY = "ed15e2c2-0934-4b6b-bb5e-10e0dbc43c2c";
        private readonly string REFRESHTOKENKEY = "ed1565e2c2-0934-4b6b-bb5e-10e0545dbc43c2c";
        private readonly string EXPTOKENKEY = "61ae0e2a-40cc-46ce-bd73-d041c491ac2d";
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;
        private readonly NavigationManager _navigationManager;
        private JsonSerializerOptions defaultJsonSerializerOptions => new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        public HttpService(HttpClient httpClient, IJSRuntime js, NavigationManager navigationManager)
        {
            _httpClient = httpClient;
            _navigationManager = navigationManager;
            _js = js;
        }
        public async Task<T> SendAsync<T>(APIRequest apiRequest)
        {
            HttpResponseMessage apiResponse = null;


            try
            {
                var token = await _js.GetFromLocalStorage(TOKENKEY);
                var renewToken = await _js.GetFromLocalStorage(REFRESHTOKENKEY);
                if ((string.IsNullOrEmpty(renewToken) || string.IsNullOrEmpty(token)) && apiRequest.CheckTokens == true)
                {
                    throw new Exception("Tokens are missing");
                }

               
                var URL = SD.BaseUrl + apiRequest.Url;
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

                var response = await Deserialize<T>(apiResponse, defaultJsonSerializerOptions);
                return response;
       
            }
            catch (Exception e)
            {
                var temp = new APIResponse<T>();
                if(apiResponse is not null)
                {
                    var content = await apiResponse.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(content))
                    {
                        temp.ErrorMessages.Add(content);
                    }
                    temp.ErrorMessages.Add(e.StackTrace);
                    temp.StatusCode = apiResponse.StatusCode;

                    if (apiResponse.StatusCode == System.Net.HttpStatusCode.MethodNotAllowed)
                    {
                        temp.ErrorMessages.Add("Endpoint not found");
                    }
                    if (apiResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        _navigationManager.NavigateTo("/logout");
                    }


                }

                if(e.Message == "Tokens are missing")
                {
                    temp.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    temp.ErrorMessages.Add(e.Message);
                    _navigationManager.NavigateTo("/logout");
                }

                
     
          
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
