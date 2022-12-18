using Microsoft.Extensions.Logging;
using MudTemplate.Server.Helpers.Interfaces;
using MudTemplate.Shared.Models;
using Newtonsoft.Json;
using System.Net;

namespace MudTemplate.Server.Helpers.Middleware
{
    public class ErrorWrappingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorWrappingMiddleware> _logger;
        private readonly ILogging _customeLogger;

        public ErrorWrappingMiddleware(RequestDelegate next, ILogger<ErrorWrappingMiddleware> logger, ILogging customLogger)
        {
            _next = next;
            _logger = logger;
            _customeLogger = customLogger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {

                _logger.LogError((EventId)0, ex, ex.Message);
                _customeLogger.Log(ex.Message,"error");
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = new GeneralAPIResponse
            {
                StatusCode = (HttpStatusCode)context.Response.StatusCode,
                IsSuccess = false,
                ErrorMessages = new List<string> { ex.Message }
            };
            var json = JsonConvert.SerializeObject(response);


            return context.Response.WriteAsync(json);
        }
    }
}
