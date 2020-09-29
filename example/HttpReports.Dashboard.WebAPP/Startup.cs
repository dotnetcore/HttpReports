using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks; 
using HttpReports.Core.Diagnostics; 
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

namespace HttpReports.Dashboard.WebAPP
{
    public class Startup
    { 

        public void ConfigureServices(IServiceCollection services) 
        {
            //services.AddHttpReports().UseHttpTransport(); 

            services.AddHttpReportsDashboard().UsePostgreSQLStorage(); 

            services.AddCors(c =>
            { 
                c.AddPolicy("Policy", policy =>
                {
                    policy.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod(); 

                });

            }); 

        } 
      
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("Policy"); 

            //app.UseHttpReports();
            app.UseHttpReportsDashboard(); 

            MapRoute(app);
        }

        public void MapRoute(IApplicationBuilder app)
        {
            //string address = "http://moa.hengyinfs.com";

            string address = "http://localhost:5010";  
           

            app.Map("/HttpClient", builder =>
            {
                builder.Run(async context =>  
                {
                     
                    HttpClient client = new HttpClient();
                    var response = client.GetStringAsync("http://www.baidu.com").Result;

                    await context.Response.WriteAsync("OK"); 

                });

            });



            app.Map("/Trace", builder =>
            {
                builder.Run(async context =>
                {
                    //System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    //HttpClient client = new HttpClient();
                    //var response = await client.GetStringAsync(address +"/Test1");
                    //await context.Response.WriteAsync(response);

                    System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(address + "/Test1");

                    req.Timeout = 5000000;

                    using (System.Net.WebResponse wr = await req.GetResponseAsync()) 
                    {
                        await context.Response.WriteAsync("ok");
                    }  

                });

            });

            app.Map("/Test1", builder =>
            { 
                builder.Run(async context =>
                { 
                    System.Threading.Thread.Sleep(new Random().Next(111,5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync(address + "/Test2");
                    await context.Response.WriteAsync(response);

                });

            });

            app.Map("/Test2", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync(address + "/Test3");
                    await context.Response.WriteAsync(response);

                });

            });

            app.Map("/Test3", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync(address + "/Test4");
                    await context.Response.WriteAsync(response);

                });

            });


            app.Map("/Test4", builder =>
            {
                builder.Run(async context =>
                {
                    System.Threading.Thread.Sleep(new Random().Next(111, 5555));

                    HttpClient client = new HttpClient();
                    var response = await client.GetStringAsync(address + "/Test5");
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
