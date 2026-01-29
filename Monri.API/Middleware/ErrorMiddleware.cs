using Microsoft.AspNetCore.Mvc;
using System.Net;
using Newtonsoft.Json;

namespace Monri.API.Middleware
{
    public class ErrorMiddleware
    {
        private const string InternalServerError = "Internal Server Error";
        private const string ActionKey = "action";

        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var action = context.Request.RouteValues[ActionKey];
                _logger.LogCritical($"{action} Error: {ex.StackTrace}");
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = JsonConvert.SerializeObject(new ProblemDetails() { Title = InternalServerError, Status = (int)code });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;

            await context.Response.WriteAsync(result);
        }
    }
}
