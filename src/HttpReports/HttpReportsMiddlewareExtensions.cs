using System;

using HttpReports;
using HttpReports.RequestInfoBuilder;
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
            services.AddSingleton<IRequestInfoBuilder, DefaultRequestInfoBuilder>();

            services.AddMvcCore(x => { 
                x.Filters.Add<HttpReportsExceptionFilter>();
            });


            return new HttpReportsBuilder(services, configuration);
        } 
 
        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<HttpReportsOptions>>();

            if (!options.Value.Open) 
                return app; 

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Service Not Found");
            storage.InitAsync().Wait(); 

            return app.UseMiddleware<DefaultHttpReportsMiddleware>();
        }  
    }
}