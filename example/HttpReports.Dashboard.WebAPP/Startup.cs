using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            services.AddHttpReportsDashboard().UseSQLServerStorage();
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
                    await context.Response.WriteAsync("Hello World!");

                });
            });

            MapRoute(app);
        }

        public void MapRoute(IApplicationBuilder app)
        {
            app.Map("/Trace", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync("http://localhost:" + context.Connection.LocalPort + "/Test1");
                    await context.Response.WriteAsync(response);

                });

            });

            app.Map("/Test1", builder =>
            { 
                builder.Run(async context =>
                { 
                    System.Threading.Thread.Sleep(new Random().Next(111,5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync("http://localhost:"+context.Connection.LocalPort+"/Test2");
                    await context.Response.WriteAsync(response);

                });

            });

            app.Map("/Test2", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync("http://localhost:" + context.Connection.LocalPort + "/Test3");
                    await context.Response.WriteAsync(response);

                });

            });

            app.Map("/Test3", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync("http://localhost:" + context.Connection.LocalPort + "/Test4");
                    await context.Response.WriteAsync(response);

                });

            });


            app.Map("/Test4", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync("http://localhost:" + context.Connection.LocalPort + "/Test5");
                    await context.Response.WriteAsync(response);

                });

            });


            app.Map("/Test5", builder => {

                builder.Run(async context => {

                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    await context.Response.WriteAsync("ok");

                    return;

                });

            });

        }

    }
}
