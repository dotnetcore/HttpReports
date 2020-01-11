using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace HttpReports
{
    internal class DefaultHttpInvokeProcesser : IHttpInvokeProcesser
    {
        public IHttpReportsStorage Storage { get; }
        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public DefaultHttpInvokeProcesser(IHttpReportsStorage storage, IRequestInfoBuilder requestInfoBuilder)
        {
            Storage = storage;
            RequestInfoBuilder = requestInfoBuilder;
        }

        public void Process(HttpContext context, Stopwatch stopwatch)
        {
            var requestInfo = RequestInfoBuilder.Build(context, stopwatch);
            //TODO 保存请求信息
        }
    }
}
