using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    internal class DefaultHttpInvokeProcesser : IHttpInvokeProcesser
    {
        public IReportsTransport ReportsTransport { get; }
        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IConfiguration Configuration { get; }

        public DefaultHttpInvokeProcesser(IReportsTransport reportsTransport, IRequestInfoBuilder requestInfoBuilder, IConfiguration configuration)
        {
            ReportsTransport = reportsTransport;
            RequestInfoBuilder = requestInfoBuilder;
            Configuration = configuration;
        }

        public void Process(HttpContext context, Stopwatch stopwatch)
        {
            var (requestInfo, requestDetail) = RequestInfoBuilder.Build(context, stopwatch);

            if (requestInfo != null && requestDetail != null)
            {
                ReportsTransport.Write(requestInfo, requestDetail);
            }
        }
    }
}