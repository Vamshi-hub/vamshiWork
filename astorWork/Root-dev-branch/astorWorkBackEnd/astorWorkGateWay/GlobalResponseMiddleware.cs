using astorWorkShared.GlobalResponse;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace astorWorkGateWay
{
    public class GlobalResponseMiddleware
    {
        private readonly RequestDelegate next;

        private readonly JsonSerializerSettings jsonSettings =
            new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

        public GlobalResponseMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                Stream originBody = context.Response.Body;

                MemoryStream newBody = new MemoryStream();
                context.Response.Body = newBody;

                string modifiedJson = "";

                //Continue down the Middleware pipeline, eventually returning to this class
                await next(context);

                modifiedJson = FormatJson(newBody);

                context.Response.Body = originBody;
                context.Response.ContentLength = Encoding.UTF8.GetByteCount(modifiedJson);

                await context.Response.WriteAsync(modifiedJson);
            }
            else
                await next(context);
        }

        private string FormatJson(MemoryStream newBody)
        {
            newBody.Seek(0, SeekOrigin.Begin);
            string json = new StreamReader(newBody).ReadToEnd();

            try
            {
                var response = JsonConvert.DeserializeObject<APIResponse>(json);
                if (response != null && (response.Data != null || response.Message != null))
                    return json;
            }
            catch (Exception exc) { }
            return JsonConvert.SerializeObject(new APIResponse(0, JsonConvert.DeserializeObject(json)), jsonSettings);
        }
    }

    public static class GlobalResponseMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalResponse(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalResponseMiddleware>();
        }
    }
}
