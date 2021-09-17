using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Diagnostics;
using HttpReports.Diagnostic.AspNetCore;
using HttpReports.Diagnostic.HttpClient;
using HttpReports.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Snowflake.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpReportsMiddlewareExtensions
    {
        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, IConfiguration configuration = null)
        {
            if (configuration != null)
            {
                return services.AddHttpReportsService(configuration.GetSection("HttpReports"));
            }
            else
            {
                var config = services.BuildServiceProvider().GetService<IConfiguration>();

                if (config == null)
                {
                    throw new ArgumentNullException(nameof(configuration));
                }

                services.AddOptions().Configure<HttpReportsOptions>(config.GetSection("HttpReports"));

                return services.AddHttpReportsService(config.GetSection("HttpReports"));

            }  
        }

        public static IHttpReportsBuilder AddHttpReports(this IServiceCollection services, Action<HttpReportsOptions> options, IConfiguration configuration = null)
        { 
            IConfiguration config = configuration ?? services.BuildServiceProvider().GetService<IConfiguration>() ?? 
                throw new ArgumentNullException(nameof(configuration));

            services.AddOptions().Configure(options); 
          
            return services.AddHttpReportsService(config.GetSection("HttpReports"));
        } 

        private static IHttpReportsBuilder AddHttpReportsService(this IServiceCollection services, IConfiguration configuration)
        {
            services.PostConfigure<HttpReportsOptions>(x =>
            {
                x.Server = services.GetNewServer(x, configuration);
            });

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };

            services.AddSingleton(jsonSerializerOptions);

            services.AddSingleton<IdWorker>(new IdWorker(new Random().Next(1, 30), new Random().Next(1, 30)));
            services.AddSingleton<IRequestProcesser, DefaultRequestProcesser>();
            services.AddSingleton<IRequestBuilder, DefaultRequestBuilder>();
            services.AddSingleton<HttpReportsBackgroundService>();
            services.AddSingleton<IPerformanceService, PerformanceService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDiagnosticListener, HttpClientDiagnosticListener>();
            services.AddSingleton<IDiagnosticListener, AspNetCoreDiagnosticListener>();
            services.AddSingleton<ISegmentContext, SegmentContext>();
            services.AddSingleton<TraceDiagnsticListenerObserver>();

            return new HttpReportsBuilder(services, configuration);
        }

        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        {
            ServiceContainer.Provider = app.ApplicationServices.GetRequiredService<IServiceProvider>();

            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            var backgroundService = app.ApplicationServices.GetRequiredService<HttpReportsBackgroundService>();
            backgroundService.StartAsync().Wait();

            var options = app.ApplicationServices.GetRequiredService<IOptions<HttpReportsOptions>>();

            if (!options.Value.Switch) return null;

            app.Map(BasicConfig.HttpReportsDefaultHealth, builder =>
            {
                builder.Run(async context => await context.Response.WriteAsync(DateTime.Now.ToShortTimeString()));
            });

            app.UseMiddleware<DefaultHttpReportsMiddleware>();

            TraceDiagnsticListenerObserver observer = app.ApplicationServices.GetRequiredService<TraceDiagnsticListenerObserver>();

            DiagnosticListener.AllListeners.Subscribe(observer);

            return app;
        }

        private static string GetNewServer(this IServiceCollection services, HttpReportsOptions options, IConfiguration configuration)
        {
            string Default = "http://localhost:5000";

            if (!options.Server.IsEmpty()) return options.Server;

            var urls = services.BuildServiceProvider().GetService<IConfiguration>()[WebHostDefaults.ServerUrlsKey] ?? configuration["server.urls"] ?? configuration["server"];

            if (!urls.IsEmpty())
            {
                var first = urls.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                if (!first.IsEmpty()) return first;
            }

            return Default;
        }
    }
}
