using System;

using HttpReports;
using HttpReports.Transport.Grpc;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGrpcReportsTransport(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                    .Configure<GrpcReportsTransportOptions>(configuration);
            return services.AddGrpcReportsTransport();
        }

        public static IServiceCollection AddGrpcReportsTransport(this IServiceCollection services, Action<GrpcReportsTransportOptions> options)
        {
            services.AddOptions()
                    .Configure(options);
            return services.AddGrpcReportsTransport();
        }

        public static IServiceCollection AddGrpcReportsTransport(this IServiceCollection services)
        {
            services.AddSingleton<IReportsTransport, GrpcReportsTransport>();
            return services;
        }
    }
}