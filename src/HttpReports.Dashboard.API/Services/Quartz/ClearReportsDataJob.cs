using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Quartz;

namespace HttpReports.Dashboard.Services.Quartz
{
    public class ClearReportsDataJob : IJob
    {
        private IHttpReportsStorage _storage;

        private DashboardAPIOptions _options;

        public ClearReportsDataJob(IHttpReportsStorage storage, IOptions<DashboardAPIOptions> options)
        {
            _storage = storage;
            _options = options.Value;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _storage.ClearData(DateTime.Now.AddDays(-_options.ExpireDay).ToString("yyyy-MM-dd"));
        }
    }
}