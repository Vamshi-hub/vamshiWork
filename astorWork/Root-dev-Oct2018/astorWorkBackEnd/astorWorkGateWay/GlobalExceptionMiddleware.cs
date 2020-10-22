using astorWorkShared.GlobalResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkGateWay
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            Console.WriteLine("Unhandled exception: " + exception.Message);
            Console.Write(exception.StackTrace);

            int code = 500; // 500 if unexpected
            string message = "An unkown error happened";

            if (exception is UnauthorizedAccessException) {
                code = 401;
                message = exception.Message;
            }

            var result = JsonConvert.SerializeObject(new { status = code, message });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            context.Response.StatusCode = 200;

            return context.Response.WriteAsync(result, Encoding.UTF8);
        }
    }

    public static class GlobalExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalException(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}
