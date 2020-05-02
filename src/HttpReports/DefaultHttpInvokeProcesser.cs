using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
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
            var (requestInfo,requestDetail) = RequestInfoBuilder.Build(context, stopwatch);

            if (requestInfo != null)
            {
                Task.Run(() => { Storage.AddRequestInfoAsync(requestInfo,requestDetail); });
            }  
        }
    }
}