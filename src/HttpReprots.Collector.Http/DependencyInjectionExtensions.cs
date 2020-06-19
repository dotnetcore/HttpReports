using HttpReports;
using HttpReprots.Collector.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsBuilder AddHttpReportsHttpCollector(this IServiceCollection services)
        { 
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");

            services.AddSingleton<HttpCollectorService>();

            return new HttpReportsBuilder(services, configuration);
        } 
        public static IApplicationBuilder UseHttpReportsHttpCollector(this IApplicationBuilder app)
        {
            var service = app.ApplicationServices.GetRequiredService<HttpCollectorService>();

            app.Map("/HttpReportsCollect",builder => {

                app.Run(async context => {

                    await context.Response.WriteAsync("Success");  

                }); 
            
            }); 

            return app;
        } 
        
    }
}
