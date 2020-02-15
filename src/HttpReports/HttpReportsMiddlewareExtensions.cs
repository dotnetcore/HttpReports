using System;

using HttpReports;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpReportsMiddlewareExtensions
    {  
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");
            return services.AddHttpReports(configuration);
        } 
        
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration)
        {  
            services.AddOptions();
            services.Configure<HttpReportsOptions>(configuration); 

            services.AddSingleton<IModelCreator, DefaultModelCreator>();
            services.AddSingleton<IHttpInvokeProcesser, DefaultHttpInvokeProcesser>();

            return new HttpReportsBuilder(services, configuration);
        }

       
        public static IHttpReportsBuilder UseAPI(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IRequestInfoBuilder, ApiRequestInfoBuilder>();
            return builder;
        }

         
        public static IHttpReportsBuilder UseWeb(this IHttpReportsBuilder builder)
        {
            builder.Services.AddSingleton<IRequestInfoBuilder, WebRequestInfoBuilder>();
            return builder;
        } 
 
        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        { 
            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Service Not Found");
            storage.InitAsync().Wait();

            return app.UseMiddleware<DefaultHttpReportsMiddleware>();
        }  
    }
}