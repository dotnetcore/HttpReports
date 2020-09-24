using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Options;
using Quartz;
using System; 
using System.Threading.Tasks;
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

namespace HttpReports.Dashboard.Services 
{
    public class ClearReportsDataJob : IJob
    {
        private IHttpReportsStorage _storage;

        private DashboardOptions _options;

        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _options = _options ?? (ServiceContainer.provider.GetService(typeof(IOptions<DashboardOptions>)) as IOptions<DashboardOptions>).Value; 

            await _storage.ClearData(DateTime.Now.AddDays(-_options.ExpireDay).ToString("yyyy-MM-dd"));

        }

    }
}
