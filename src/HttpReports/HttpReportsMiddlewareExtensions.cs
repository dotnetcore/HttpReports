using System; 
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Diagnostics;
using HttpReports.Diagnostic.HttpClient; 
using HttpReports.RequestInfoBuilder;
using HttpReports.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
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
            services.AddSingleton<IHttpInvokeProcesser, DefaultHttpInvokeProcesser>();
            services.AddSingleton<IRequestInfoBuilder, DefaultRequestInfoBuilder>();
            services.AddSingleton<IBackgroundService, HttpReportsBackgroundService>();
            services.AddSingleton<IPerformanceService,PerformanceService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IDiagnosticListener, HttpClientDiagnosticListener>(); 
            services.AddSingleton<ISegmentContext, SegmentContext>();
            services.AddSingleton<TraceDiagnsticListenerObserver>();  

            services.AddMvcCore(x =>
            {
                x.Filters.Add<HttpReportsExceptionFilter>();

            });

            return new HttpReportsBuilder(services, configuration).UseDirectlyReportsTransport();
        }

        public static IHttpReportsInitializer UseHttpReports(this IApplicationBuilder app)
        { 
            ServiceContainer.Provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Faild"); 

            Activity.DefaultIdFormat = ActivityIdFormat.W3C; 

            IHttpReportsInitializer httpReportsInitializer = app.InitHttpReports().InitStorage();

            var backgroundService = app.ApplicationServices.GetRequiredService<IBackgroundService>();  
            backgroundService.StartAsync(app); 

            var options = app.ApplicationServices.GetRequiredService<IOptions<HttpReportsOptions>>(); 

            if (!options.Value.Switch) return null; 

            app.UseMiddleware<DefaultHttpReportsMiddleware>(); 

            TraceDiagnsticListenerObserver observer = app.ApplicationServices.GetRequiredService<TraceDiagnsticListenerObserver>();  

            System.Diagnostics.DiagnosticListener.AllListeners.Subscribe(observer);  

            return httpReportsInitializer;
        }
    }
}