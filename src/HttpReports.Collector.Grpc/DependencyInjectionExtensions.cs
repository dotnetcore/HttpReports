using HttpReports.Collector.Grpc;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddHttpReportsGrpcCollector(this IServiceCollection services)
        {
            services.AddGrpc();
            return services;
        }

        public static GrpcServiceEndpointConventionBuilder MapHttpReportsGrpcCollector(this IEndpointRouteBuilder builder)
        {
            return builder.MapGrpcService<GrpcCollectorService>();
        }
    }
}