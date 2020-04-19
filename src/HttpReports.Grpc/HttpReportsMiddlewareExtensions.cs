using System;
using System.Diagnostics;
using HttpReports;
using HttpReports.Grpc;
using HttpReports.RequestInfoBuilder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class HttpReportsMiddlewareExtensions
    {   
        public static IHttpReportsBuilder UseGrpc(this IHttpReportsBuilder builder)
        { 
            builder.Services.AddGrpc(x => {   x.Interceptors.Add<HttpReportsGrpcLoggerInterceptor>();   });

            return builder;
        }   
        
    }
}