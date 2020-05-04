using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    internal class DefaultHttpInvokeProcesser : IHttpInvokeProcesser
    {
        public IReportsDataWriter ReportsDataWriter { get; }
        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IConfiguration Configuration { get; }

        public DefaultHttpInvokeProcesser(IReportsDataWriter reportsDataWriter, IRequestInfoBuilder requestInfoBuilder, IConfiguration configuration)
        {
            ReportsDataWriter = reportsDataWriter;
            RequestInfoBuilder = requestInfoBuilder;
            Configuration = configuration;
        }

        public void Process(HttpContext context, Stopwatch stopwatch)
        {
            var (requestInfo, requestDetail) = RequestInfoBuilder.Build(context, stopwatch);

            if (requestInfo != null)
            {
                ReportsDataWriter.Write(requestInfo, requestDetail);
            }
        }
    }
}