using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Storage.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task WriteDataAsync(RequestBag bag)
        { 
            await Storage.AddRequestInfoAsync(bag);  
        }

        public async Task WriteDataAsync(Performance performance)
        {
            await Storage.AddPerformanceAsync(performance); 
        } 
    }
}
