using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using MudTemplate.Backend;
using MudTemplate.Backend.Repositories;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Server.Helpers.Logging;
using MudTemplate.Server.Helpers.Middleware;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Utilities;
using MudTemplate.Shared.Utilities.IUtilities;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddProjectServices();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.Configure<ApiBehaviorOptions>(options
    => options.SuppressModelStateInvalidFilter = true);

//Add Db
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Logging
Log.Logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File("Log/apiLogs.txt",rollingInterval:RollingInterval.Month).WriteTo.Console().CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddSingleton<ILogging, Logging>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
    });
}
else
{
    app.UseExceptionHandler("/Error");

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}




app.CustomExceptionMiddleware();
app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
