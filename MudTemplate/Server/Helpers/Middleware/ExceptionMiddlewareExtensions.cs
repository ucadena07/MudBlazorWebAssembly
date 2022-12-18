namespace MudTemplate.Server.Helpers.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void CustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorWrappingMiddleware>();
        }
    }
}
