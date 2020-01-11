using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    internal class DefaultHttpInvokeProcesser : IHttpInvokeProcesser
    {
        public IHttpReportsStorage Storage { get; }
        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IConfiguration Configuration;

        public DefaultHttpInvokeProcesser(IHttpReportsStorage storage, IRequestInfoBuilder requestInfoBuilder, IConfiguration configuration)
        {
            Storage = storage;
            RequestInfoBuilder = requestInfoBuilder;
            Configuration = configuration;
        }

        public void Process(HttpContext context, Stopwatch stopwatch)
        {
            var requestInfo = RequestInfoBuilder.Build(context, stopwatch);
            //TODO 保存请求信息
        }
    }
}
