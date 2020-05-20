using HttpReports;
using HttpReports.Collector.Grpc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IHttpReportsBuilder AddHttpReportsGrpcCollector(this IHttpReportsBuilder builder)
        {
            builder.Services.AddGrpc();
            return builder;
        }

        public static GrpcServiceEndpointConventionBuilder MapHttpReportsGrpcCollector(this IEndpointRouteBuilder builder)
        {
            return builder.MapGrpcService<GrpcCollectorService>();
        }
    }
}