using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudTemplate.Client;
using MudTemplate.Client.Auth;
using MudTemplate.Client.Services;
using MudTemplate.Client.Services.Http;
using MudTemplate.Shared.IServices;
using MudTemplate.Shared.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
SD.BaseUrl = new Uri(builder.HostEnvironment.BaseAddress).ToString();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<TokenRenewer>();

//Auth to client side
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>(provider =>
    provider.GetRequiredService<JwtAuthenticationStateProvider>()
); ;
builder.Services.AddScoped<ILoginService, JwtAuthenticationStateProvider>(provider => 
    provider.GetRequiredService<JwtAuthenticationStateProvider>()   
);

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
