using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudTemplate.Client;
using MudTemplate.Client.Services;
using MudTemplate.Client.Services.Http;
using MudTemplate.Client.Services.IServices;
using MudTemplate.Shared.Utilities;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
SD.BaseUrl = new Uri(builder.HostEnvironment.BaseAddress).ToString();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<ITestService, TestService>();



await builder.Build().RunAsync();
