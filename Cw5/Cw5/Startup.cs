
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cw5.Middlewares;
using Cw5.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cw5
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IStudentsDbService, SqlServerDbService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IStudentsDbService dbService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LoggingMiddleware>();
            app.Use(async (context, next) =>
            {

                if (!context.Request.Headers.ContainsKey("Index") ||
                !dbService.IsStudentExists(context.Request.Headers["Index"]))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Nie poda³eœ indeksu");
                    return;
                }
                await next();
            });

            app.UseRouting();  // /api/students/10/grades GET   -->  StudentsController i GetStudents

            app.UseAuthorization();

            app.UseEndpoints(endpoints => // Wykonuje zadania GetStudents()
            {
                endpoints.MapControllers();
            });
        }
    }
}