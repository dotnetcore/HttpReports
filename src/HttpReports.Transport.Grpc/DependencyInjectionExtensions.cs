using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;  

namespace HttpReports.Transport.Grpc
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder)
        {
            builder.Services.AddOptions().Configure<GrpcTransportOptions>(builder.Configuration.GetSection("Transport"));
            return builder.UseHttpTransportService();
        }

        public static IHttpReportsBuilder UseHttpTransport(this IHttpReportsBuilder builder, Action<GrpcTransportOptions> options)
        {
            builder.Services.AddOptions().Configure(options);
            return builder.UseHttpTransportService();
        }

        private static IHttpReportsBuilder UseHttpTransportService(this IHttpReportsBuilder builder)
        { 
            builder.Services.RemoveAll<IReportsTransport>();
            builder.Services.AddSingleton<IReportsTransport,GrpcTransport>();
            return builder;
        }
    }
}
