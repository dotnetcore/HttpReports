using System;
using System.Diagnostics;

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
            services.AddOptions();
            services.Configure<HttpReportsOptions>(configuration);
            return services.AddHttpReportsService(configuration);
        }

        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, Action<HttpReportsOptions> options)
        {
            IConfiguration configuration = services.BuildServiceProvider().GetService<IConfiguration>().GetSection("HttpReports");
            services.AddOptions();
            services.Configure<HttpReportsOptions>(options);

            return services.AddHttpReportsService(configuration);
        }

        private static IHttpReportsBuilder AddHttpReportsService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IModelCreator, DefaultModelCreator>();
            services.AddSingleton<IHttpInvokeProcesser, DefaultHttpInvokeProcesser>();
            services.AddSingleton<IReportsDataWriter, DirectlyReportsDataWriter>();
            services.AddSingleton<IRequestInfoBuilder, DefaultRequestInfoBuilder>();

            services.AddMvcCore(x =>
            {
                x.Filters.Add<HttpReportsExceptionFilter>();
            });

            return new HttpReportsBuilder(services, configuration);
        }

        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            var options = app.ApplicationServices.GetRequiredService<IOptions<HttpReportsOptions>>();

            if (!options.Value.Switch)
                return app;

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Service Not Found");
            storage.InitAsync().Wait();

            return app.UseMiddleware<DefaultHttpReportsMiddleware>();
        }
    }
}