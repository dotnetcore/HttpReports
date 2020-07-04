using HttpReports;
using HttpReports.Core;
using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReprots.Collector.Http
{
    public class HttpCollectorService
    {
        public IHttpReportsStorage Storage { get; }

        public HttpCollectorService(IHttpReportsStorage storage)
        {
            Storage = storage;
        }

        public async Task<string> Write(List<RequestBag> list)
        {
            foreach (var item in list)
            {
                await Storage.AddRequestInfoAsync(item);
            }

            return "Ok";
        }

        public async Task<string> WritePerformance(Performance performance)
        {
            await Storage.AddPerformanceAsync(performance);

            return "Ok";
        }  
    }
}
