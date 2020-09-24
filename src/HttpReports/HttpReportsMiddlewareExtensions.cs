using System; 
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks; 
using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Diagnostics;
using HttpReports.Diagnostic.AspNetCore;
using HttpReports.Diagnostic.HttpClient;  
using HttpReports.Services; 
using Microsoft.AspNetCore.Builder; 
using Microsoft.AspNetCore.Http; 
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
            services.AddSingleton<IRequestProcesser, DefaultRequestProcesser>();
            services.AddSingleton<IRequestBuilder, DefaultRequestBuilder>();
            services.AddSingleton<IBackgroundService, HttpReportsBackgroundService>();
            services.AddSingleton<IPerformanceService,PerformanceService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDiagnosticListener, HttpClientDiagnosticListener>();
            services.AddSingleton<IDiagnosticListener, AspNetCoreDiagnosticListener>();
            services.AddSingleton<ISegmentContext, SegmentContext>();
            services.AddSingleton<TraceDiagnsticListenerObserver>();

            return new HttpReportsBuilder(services, configuration);
        }

        public static IApplicationBuilder UseHttpReports(this IApplicationBuilder app)
        { 
            ServiceContainer.Provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Failed"); 

            Activity.DefaultIdFormat = ActivityIdFormat.W3C; 

            var backgroundService = app.ApplicationServices.GetRequiredService<IBackgroundService>();  
            backgroundService.StartAsync(app); 

            var options = app.ApplicationServices.GetRequiredService<IOptions<HttpReportsOptions>>(); 

            if (!options.Value.Switch) return null; 

            app.UseMiddleware<DefaultHttpReportsMiddleware>(); 

            TraceDiagnsticListenerObserver observer = app.ApplicationServices.GetRequiredService<TraceDiagnsticListenerObserver>();  

            DiagnosticListener.AllListeners.Subscribe(observer);

            return app;
        }
    }
}