using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.JSInterop;
using MudTemplate.Components.Helpers.JsExtensions;
using MudTemplate.Shared.IServices;
using MudTemplate.Shared.Models;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace MudTemplate.Client.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider, ILoginService
    {
        private readonly IJSRuntime _js;
        private readonly string TOKENKEY = "ed15e2c2-0934-4b6b-bb5e-10e0dbc43c2c";
        private readonly string REFRESHTOKENKEY = "ed1565e2c2-0934-4b6b-bb5e-10e0545dbc43c2c";
        private readonly string EXPTOKENKEY = "61ae0e2a-40cc-46ce-bd73-d041c491ac2d";
        private readonly HttpClient _httpClient;
        private readonly IAccountService _accountService;
        private readonly NavigationManager _navigationManager;

        private AuthenticationState Anonymous => new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        public JwtAuthenticationStateProvider(IJSRuntime js, HttpClient httpClient, IAccountService accountService, NavigationManager navigationManager)
        {
            _js = js;
            _httpClient = httpClient;
            _accountService = accountService;
            _navigationManager = navigationManager; 
        }

        public async Task Login(UserToken userToken)
        {
            await _js.SetInLocalStorage(TOKENKEY, userToken.Token);
            await _js.SetInLocalStorage(REFRESHTOKENKEY, userToken.RefreshToken);
            await _js.SetInLocalStorage(EXPTOKENKEY, userToken.Expiration.ToString());
            var authState = BuildAuthenticationState(userToken.Token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public async Task Logout()
        {
            await CleanUp();
            NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
            _navigationManager.NavigateTo("/");
        }

        public async Task TryRenewToken()
        {
            var expTimeString = await _js.GetFromLocalStorage(EXPTOKENKEY);
            DateTime expTime;

            if (DateTime.TryParse(expTimeString, out expTime))
            {
                if (isTokenExpired(expTime))
                {
                    await Logout();

                }

                if (ShouldRenewToken(expTime))
                {
                    var token = await _js.GetFromLocalStorage(TOKENKEY);
                    var renewToken = await _js.GetFromLocalStorage(REFRESHTOKENKEY);
                    var newToken = await RenewToken(token,renewToken);
                    var authState = BuildAuthenticationState(newToken);
                    NotifyAuthenticationStateChanged(Task.FromResult(authState));
                }
            }
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _js.GetFromLocalStorage(TOKENKEY);
            var renewToken = await _js.GetFromLocalStorage(REFRESHTOKENKEY);

            if (string.IsNullOrEmpty(token))
            {
                return Anonymous;
            }

            var expTimeString = await _js.GetFromLocalStorage(EXPTOKENKEY);
            DateTime expTime;

            if (DateTime.TryParse(expTimeString, out expTime))
            {
                if (isTokenExpired(expTime))
                {
                    await CleanUp();
                    return Anonymous;
                }

                if (ShouldRenewToken(expTime))
                {
                    token = await RenewToken(token,renewToken);
                }
            }


            return BuildAuthenticationState(token);
        }
        private async Task<string> RenewToken(string token, string refreshToken)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            var apiResp = await _accountService.RenewToken<APIResponse<AuthResponse>>(new RefreshTokenRequest { ExpiredToken = token,RefreshToken = refreshToken});
            var newToken = apiResp.Result;
            await _js.SetInLocalStorage(TOKENKEY, newToken.Token);
            await _js.SetInLocalStorage(REFRESHTOKENKEY, newToken.RefreshToken);
            await _js.SetInLocalStorage(EXPTOKENKEY, newToken.TokenExpDate.ToString());
            return newToken.Token;
        }

        private bool ShouldRenewToken(DateTime expirationTime)
        {
            return expirationTime.Subtract(DateTime.UtcNow) < TimeSpan.FromMinutes(2);
        }

        private bool isTokenExpired(DateTime expirationTime)
        {
            return expirationTime <= DateTime.UtcNow;
        }


        private AuthenticationState BuildAuthenticationState(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt")));

        }

        private async Task CleanUp()
        {
            await _js.RemoveItem(TOKENKEY);
            await _js.RemoveItem(EXPTOKENKEY);
            await _js.RemoveItem(REFRESHTOKENKEY);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            keyValuePairs.TryGetValue(ClaimTypes.Role, out object roles);

            if (roles != null)
            {
                if (roles.ToString().Trim().StartsWith("["))
                {
                    var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                    foreach (var parsedRole in parsedRoles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, parsedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            return claims;
        }
    }
}
