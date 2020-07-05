using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Config;
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
        public static IServiceCollection AddHttpReportsHttpCollector(this IServiceCollection services)
        { 
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");

            services.AddSingleton<IHttpReportsCollector,HttpCollectorService>();

            return services;

        } 
        public static IApplicationBuilder UseHttpReportsHttpCollector(this IApplicationBuilder app)
        { 
            app.Map(BasicConfig.TransportPath, builder => {

                builder.UseMiddleware<HttpCollectorMiddleware>();  

            }); 

            return app;
        } 
        
    }
}
