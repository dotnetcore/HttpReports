using HttpReports.Core;
using HttpReports.Core.ViewModels;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Implements;
using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ServiceContainer = HttpReports.Dashboard.Implements.ServiceContainer;

namespace HttpReports.Dashboard.Services
{

    [DisallowConcurrentExecution]
    public class HealthCheckJob : IJob
    {
        private IHttpReportsStorage _storage;

        private DashboardOptions _options;

        private IHealthCheckService _healthCheckService;
        private IHttpClientFactory _httpClientFactory;

        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _options = _options ?? (ServiceContainer.provider.GetService(typeof(IOptions<DashboardOptions>)) as IOptions<DashboardOptions>).Value;
            _healthCheckService = ServiceContainer.provider.GetService(typeof(IHealthCheckService)) as IHealthCheckService;
            _httpClientFactory = ServiceContainer.provider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;

            await CheckAsync();
        }

        public async Task CheckAsync()
        { 
            var list = await _healthCheckService.GetServiceInstance();

            var client = _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient);
            client.Timeout = TimeSpan.FromSeconds(5);

            foreach (var item in list)
            {
                foreach (var k in item.Instances)
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    try
                    {
                        var response = await client.GetAsync(k.Instance + _options.Check.Path); 
                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        {
                            k.Status = HealthStatusEnum.IsCritical;
                        }  
                        var result = await response.Content.ReadAsStringAsync(); 
                    }
                    catch (Exception ex)
                    {


                    }
                    finally { 

                        stopwatch.Stop();  
                    
                    }
                   
                }  
            } 
        } 

    }
}
