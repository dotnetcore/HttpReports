using System;

using HttpReports;
using HttpReports.Transport.Grpc;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsBuilder UseGrpcReportsTransport(this IHttpReportsBuilder builder, IConfiguration configuration)
        {
            builder.Services.AddOptions()
                            .Configure<GrpcReportsTransportOptions>(configuration);
            return builder.UseGrpcReportsTransport();
        }

        public static IHttpReportsBuilder UseGrpcReportsTransport(this IHttpReportsBuilder builder, Action<GrpcReportsTransportOptions> options)
        {
            builder.Services.AddOptions()
                            .Configure(options);
            return builder;
        }

        public static IHttpReportsBuilder UseGrpcReportsTransport(this IHttpReportsBuilder builder)
        {
            builder.Services.RemoveAll<IReportsTransport>();
            builder.Services.AddSingleton<IReportsTransport, GrpcReportsTransport>();
            builder.Services.RemoveAll<IModelCreator>();
            builder.Services.AddSingleton<IModelCreator, ModelCreator>();
            return builder;
        }
    }
}