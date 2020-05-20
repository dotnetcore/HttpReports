using System;

using HttpReports.Dashboard;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpReportsDashboardUI(this IServiceCollection services, Action<DashboardUIOptions> option)
        {
            services.AddOptions()
                    .Configure(option);
            return services;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpReportsDashboardUI(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                    .Configure<DashboardUIOptions>(configuration);
            return services;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReportsDashboardUI(this IApplicationBuilder app)
        {
            app.UseMiddleware<HttpReportsDashboardUIMiddleware>();
            return app;
        }
    }
}