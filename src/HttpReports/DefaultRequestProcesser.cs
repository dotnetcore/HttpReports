using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports 
{
    internal class DefaultRequestProcesser : IRequestProcesser
    {
        public IReportsTransport ReportsTransport { get; }

        public IRequestBuilder RequestBuilder { get; }

        public IConfiguration Configuration { get; }

        public DefaultRequestProcesser(IReportsTransport reportsTransport, IRequestBuilder requestBuilder, IConfiguration configuration)
        {
            ReportsTransport = reportsTransport;
            RequestBuilder = requestBuilder;
            Configuration = configuration;
        }

        public void Process(HttpContext context)
        {
            var (requestInfo, requestDetail) = RequestBuilder.Build(context);

            if (requestInfo != null && requestDetail != null)
            {
                ReportsTransport.SendDataAsync(new Core.RequestBag(requestInfo,requestDetail));  
            }
        }
    }
}