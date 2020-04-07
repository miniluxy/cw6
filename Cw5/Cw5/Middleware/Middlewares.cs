using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw5.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (httpContext.Request != null)
            {
                httpContext.Request.EnableBuffering();
                String Path = httpContext.Request.Path;
                String method = httpContext.Request.Method;
                String query = httpContext.Request.QueryString.ToString();
                String bodyStr = "";
                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }
                using (StreamWriter writer = new StreamWriter(@"Logs.txt", true))
                {
                    writer.WriteLine(method + " " + Path + "\r\n" + bodyStr + "\r\n" + query);
                    writer.Flush();
                }
            }
            await _next(httpContext);
        }

    }
}
