using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace HttpReports
{
    internal class DefaultHttpReportsMiddleware
    {
        private readonly RequestDelegate _next;

        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IHttpInvokeProcesser InvokeProcesser { get; }

        public DefaultHttpReportsMiddleware(RequestDelegate next, IRequestInfoBuilder requestInfoBuilder, IHttpInvokeProcesser invokeProcesser)
        {
            _next = next;
            RequestInfoBuilder = requestInfoBuilder;
            InvokeProcesser = invokeProcesser;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                if (!string.IsNullOrEmpty(context.Request.Path))
                {
                   InvokeProcesser.Process(context, stopwatch);
                }
            }
        }
    }
}