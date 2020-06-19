using HttpReports;
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

        public async Task<string> Write(Dictionary<IRequestInfo,IRequestDetail> data)
        {
            foreach (var item in data)
            {
                await Storage.AddRequestInfoAsync(item.Key, item.Value);
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
