using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HttpReports.Dashboard.WebAPP
{
    public class Startup
    {
       
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpReports().UseSQLServerStorage();

            services.AddHttpReportsDashboard();
        }

      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        { 
            app.UseHttpReports();

            app.UseHttpReportsDashboard();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            { 
                //endpoints.MapHttpReportsGrpcCollector();

                endpoints.MapGet("/Test", async context =>
                {
                    throw new Exception("error");

                    await context.Response.WriteAsync("Hello World!");

                });
            });
        }
    }
}
