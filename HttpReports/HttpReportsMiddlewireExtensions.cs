using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public static class HttpReportsMiddlewireExtensions
    {
        
        /// <summary>
        /// 添加HttpReports中间件
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection AddHttpReportsMiddlewire(this IServiceCollection services, Action<HttpReportsOptions> options = null)
        {
            if (options != null)
            {
                services.AddOptions();
                services.Configure(options); 
            }

            return services.AddTransient<IHttpReports, DefaultHttpReports>();
        }

        public static IServiceCollection AddHttpReportsMiddlewire<T>(this IServiceCollection services) where T : IHttpReports
        {
            return services.AddTransient(typeof(IHttpReports), typeof(T));
        }
         

        /// <summary>
        /// 使用 HttpReports, 必须放在UseMvc之前
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseHttpReportsMiddlewire(this IApplicationBuilder app)
        {
            return app.UseMiddleware<HttpReportsMiddlewire>();
        }
    }
}
