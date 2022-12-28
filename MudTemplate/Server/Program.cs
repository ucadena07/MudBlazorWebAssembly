using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MudTemplate.Backend;
using MudTemplate.Backend.Helpers.Interfaces;
using MudTemplate.Backend.Repositories;
using MudTemplate.Server.Helpers;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Server.Helpers.Logging;
using MudTemplate.Server.Helpers.Middleware;
using MudTemplate.Shared.IRepositories;
using MudTemplate.Shared.Utilities;
using MudTemplate.Shared.Utilities.IUtilities;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//keys 
var jwtKey = builder.Configuration.GetValue<string>("JwtSettings:Key");
var applicationId = builder.Configuration.GetValue<string>("Application:ApplicationId");
var masterKey = $"{jwtKey}:{applicationId}";
builder.Services.AddScoped<IGlobalSettings, GlobalSettings>();

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

//validate token parameters
builder.Services.AddAuthentication(it =>
{
    it.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    it.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(it =>
{
    it.RequireHttpsMetadata = false;
    it.SaveToken = true;
    it.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(masterKey)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    it.Events = new JwtBearerEvents();
    it.Events.OnTokenValidated = async (context) =>
    {
        var ipAddress = context.Request.HttpContext.Connection.RemoteIpAddress.ToString();
        var jwtService = context.Request.HttpContext.RequestServices.GetService<IJwtService>();
        var jwtToken = context.SecurityToken as JwtSecurityToken;
        if (!await jwtService.IsTokenValid(jwtToken.RawData, ipAddress))
            context.Fail("Invalid token details");
    };
});

builder.Services.AddEndpointsApiExplorer();
//Add security definition to swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id= "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

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


app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
