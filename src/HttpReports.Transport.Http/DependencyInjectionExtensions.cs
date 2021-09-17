using HttpReports;
using HttpReports.Core;
using HttpReports.Transport.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        [Obsolete("Use AddHttpTransport instead")]
        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions().Configure<HttpTransportOptions>(builder.Configuration.GetSection("Transport"));
            return builder.AddHttpTransportService();
        }

        [Obsolete("Use AddHttpTransport instead")]
        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder, Action<HttpTransportOptions> options)
        {
            builder.Services.AddOptions().Configure(options);
            return builder.AddHttpTransportService();
        }

        public static IHttpReportsBuilder AddHttpTransport(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions().Configure<HttpTransportOptions>(builder.Configuration.GetSection("Transport"));
            return builder.AddHttpTransportService();
        } 

        public static IHttpReportsBuilder AddHttpTransport(this IHttpReportsBuilder builder, Action<HttpTransportOptions> options)
        {
            builder.Services.AddOptions().Configure(options);
            return builder.AddHttpTransportService();
        }

        private static IHttpReportsBuilder AddHttpTransportService(this IHttpReportsBuilder builder)
        {
            builder.Services.AddHttpClient(BasicConfig.HttpReportsHttpClient, client =>
            {
                client.DefaultRequestHeaders.Clear();
                client.Timeout = TimeSpan.FromSeconds(5);

            });

            builder.Services.RemoveAll<IReportsTransport>();
            builder.Services.AddSingleton<IReportsTransport, HttpTransport>();
            return builder;
        }
    }
}
