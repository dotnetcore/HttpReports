using System;

using HttpReports;
using HttpReports.DataWriter.Grpc;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddGrpcReportsDataWriter(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions()
                    .Configure<GrpcReportsDataWriterOptions>(configuration);
            return services.AddGrpcReportsDataWriter();
        }

        public static IServiceCollection AddGrpcReportsDataWriter(this IServiceCollection services, Action<GrpcReportsDataWriterOptions> options)
        {
            services.AddOptions()
                    .Configure(options);
            return services.AddGrpcReportsDataWriter();
        }

        public static IServiceCollection AddGrpcReportsDataWriter(this IServiceCollection services)
        {
            services.AddSingleton<IReportsDataWriter, GrpcReportsDataWriter>();
            return services;
        }
    }
}