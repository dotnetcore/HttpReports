using Grpc.Core;
using Grpc.Core.Interceptors;
using HttpReports.Core.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Grpc
{
    public class HttpReportsGrpcLoggerInterceptor : Interceptor
    {
        private readonly ILogger<HttpReportsGrpcLoggerInterceptor> _logger;

        public HttpReportsGrpcLoggerInterceptor(ILogger<HttpReportsGrpcLoggerInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            TResponse response = null;

            try
            {
                 response = await continuation(request, context);
            }
            finally 
            {
                var httpContext = context.GetHttpContext();

                httpContext.Items.Add(BasicConfig.HttpReportsGrpcRequest,request);
                httpContext.Items.Add(BasicConfig.HttpReportsGrpcResponse,response);  
            }

            return response; 
        }
    }
}
