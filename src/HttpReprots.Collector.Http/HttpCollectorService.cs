using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReprots.Collector.Http
{
    public class HttpCollectorService: IHttpReportsCollector
    {
        public IHttpReportsStorage Storage { get; }

        public HttpCollectorService(IHttpReportsStorage storage)
        {
            Storage = storage;
        }

        public async Task WriteRequestBag(List<RequestBag> list)
        { 
            await Storage.AddRequestInfoAsync(list,new System.Threading.CancellationToken());  
        }

        public async Task WritePerformance(IPerformance performance)
        {
            await Storage.AddPerformanceAsync(performance); 
        } 
    }
}
