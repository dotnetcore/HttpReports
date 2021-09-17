using System;
using System.Linq;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using HttpReports;
using HttpReports.Core;
using HttpReports.Dashboard;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Handles;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Routes;
using HttpReports.Dashboard.Services; 
using HttpReports.Storage.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Quartz.Spi;
using Snowflake.Core;
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    { 
        public static IHttpReportsBuilder AddHttpReportsDashboard(this IServiceCollection services,IConfiguration configuration = null)
        {  
            IConfiguration config = configuration ?? services.BuildServiceProvider().GetService<IConfiguration>() ??
                throw new ArgumentNullException(nameof(configuration)); 
             
            services.AddOptions().Configure<DashboardOptions>(config.GetSection("HttpReportsDashboard"));

            return services.UseHttpReportsDashboardService(config.GetSection("HttpReportsDashboard"));

        }


        public static IHttpReportsBuilder AddHttpReportsDashboard(this IServiceCollection services,Action<DashboardOptions> options,IConfiguration configuration = null)
        {  
            IConfiguration config = configuration ?? services.BuildServiceProvider().GetService<IConfiguration>() ?? 
               throw new ArgumentNullException(nameof(configuration)); 
           
            services.AddOptions().Configure<DashboardOptions>(options);

            return services.UseHttpReportsDashboardService(config.GetSection("HttpReportsDashboard"));
        }


        private static IHttpReportsBuilder UseHttpReportsDashboardService(this IServiceCollection services, IConfiguration configuration)
        {
            services.PostConfigure<DashboardOptions>(x => {

                if (x.Check == null)
                {
                    x.Check = new HealthCheckOptions();
                }

                if (string.IsNullOrEmpty(x.Check.Endpoint))
                {
                    x.Check.Endpoint = BasicConfig.HttpReportsDefaultHealth;
                }

                if (string.IsNullOrEmpty(x.Check.Mode))
                {
                    x.Check.Mode = "Self";
                } 

                if (string.IsNullOrEmpty(x.Check.Range))
                {
                    x.Check.Range = "500,2000";
                } 

            });

            if (configuration.GetValue(nameof(DashboardOptions.EnableCors), true))
            {
                services.AddCors(c =>
                {
                    c.AddPolicy(BasicConfig.Policy, policy =>
                    {
                        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                    });
                });
            }

            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) 
            };

            jsonSerializerOptions.Converters.Add(new SnowFlakeIdConverter()); 
            services.AddSingleton(jsonSerializerOptions);

            services.AddSingleton<IdWorker>(new IdWorker(new Random().Next(1,30), new Random().Next(1,30)));
            services.AddHttpClient(BasicConfig.HttpReportsHttpClient);  
            services.AddSingleton<IAlarmService, AlarmService>(); 
            services.AddSingleton<IAuthService, AuthService>();  
            services.AddSingleton<IScheduleService, ScheduleService>();  
            services.AddSingleton<ILocalizeService,LocalizeService>();
            services.AddSingleton<IHealthCheckService, HealthCheckService>(); 
            services.AddHandleService();  

            return new HttpReportsBuilder(services, configuration);
        }   

        public static IApplicationBuilder UseHttpReportsDashboard(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<IOptions<DashboardOptions>>() ?? throw new ArgumentNullException("DashboardOptions Init Failed");

            if (options.Value.EnableCors) app.UseCors(BasicConfig.Policy); 

            ServiceContainer.provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Failed");   

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Not Found");

            if (options.Value.Migration) storage.InitAsync().Wait();

            app.ApplicationServices.GetService<IScheduleService>().InitAsync().Wait(); 

            var localizeService = app.ApplicationServices.GetRequiredService<ILocalizeService>() ?? throw new ArgumentNullException("localizeService Not Found");

            localizeService.InitAsync().Wait(); 

            app.UseHttpCollector();

            app.UseMiddleware<DashboardMiddleware>();   

            return app;
        }


        public static IApplicationBuilder UseHttpReportsMigration(this IApplicationBuilder app)
        {
            ServiceContainer.provider = app.ApplicationServices.GetRequiredService<IServiceProvider>() ?? throw new ArgumentNullException("ServiceProvider Init Failed");

            var storage = app.ApplicationServices.GetRequiredService<IHttpReportsStorage>() ?? throw new ArgumentNullException("Storage Not Found");

            storage.PrintSQLAsync().Wait();

            return app;
        
        } 


        private static IServiceCollection AddHandleService(this IServiceCollection services)
        {
            var handles = Assembly.GetAssembly(typeof(DashboardRoute)).GetTypes().Where(x => typeof(DashboardHandleBase).IsAssignableFrom(x) && x != typeof(DashboardHandleBase));

            foreach (var handle in handles)
            {
                services.AddTransient(handle);
            }

            return services;

        }

        public class SnowFlakeIdConverter : System.Text.Json.Serialization.JsonConverter<long>
        {
            public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                try
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        return reader.GetString().ToLong();
                    }
                    else
                    {
                        return reader.GetInt64();
                    }  
                }
                catch (Exception ex)
                {
                    return 0L;
                }  
            } 

            public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString()); 

        }

    }
}