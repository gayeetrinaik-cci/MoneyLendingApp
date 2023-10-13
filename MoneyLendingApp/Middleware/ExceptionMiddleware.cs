using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using MoneyLendingApp.Extensions;
using Shared.DTO;
using System.Net;
using System.Threading.Tasks;

namespace MoneyLendingApp.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch(Exception ex)
            {
                var response = httpContext.Response;
                response.ContentType = "application/json";

                var (status, message) = ((int)HttpStatusCode.InternalServerError, ex.Message);
                response.StatusCode = status;

                await response.WriteAsync(new ResponseError()
                {
                    ErrorCode = status,
                    Message = message
                }.ToJsonString());
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
