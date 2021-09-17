using HttpReports.Transport.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Collector.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddHttpReports().AddHttpTransport();
            services.Configure<HttpReportsOptions>(Configuration.GetSection("HttpReports"));
            services.Configure<HttpTransportOptions>(Configuration.GetSection("HttpReports").GetSection("Transport"));
            services.AddHttpReports().AddHttpTransport();
        }
         
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHttpReports();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
