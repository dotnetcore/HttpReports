using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    internal class DefaultHttpInvokeProcesser : IHttpInvokeProcesser
    {
        public IHttpReportsStorage Storage { get; }
        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IConfiguration Configuration { get; }

        public DefaultHttpInvokeProcesser(IHttpReportsStorage storage, IRequestInfoBuilder requestInfoBuilder, IConfiguration configuration)
        {
            Storage = storage;
            RequestInfoBuilder = requestInfoBuilder;
            Configuration = configuration;
        }

        public void Process(HttpContext context, Stopwatch stopwatch)
        {
            var requestInfo = RequestInfoBuilder.Build(context, stopwatch);

            Storage.AddRequestInfoAsync(requestInfo).ConfigureAwait(false);
        }
    }
}