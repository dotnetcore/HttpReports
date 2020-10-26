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
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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

            foreach (var item in list)
            {
                foreach (var k in item.Instances)
                {
                    k.Status = await HttpInvoke(_options.Check.Endpoint,null);

                    if (k.Status == HealthStatusEnum.IsPassing)
                    {
                        item.ServiceInfo.Passing += 1;
                    }

                    if (k.Status == HealthStatusEnum.IsWarning)
                    {
                        item.ServiceInfo.Warning += 1;
                    }

                    if (k.Status == HealthStatusEnum.IsCritical)
                    {
                        item.ServiceInfo.Critical += 1;
                    } 
                }   
            }

            await _healthCheckService.SetServiceInstance(list);   
        } 

        public async Task<HealthStatusEnum> HttpInvoke(string Endpoint, List<int> costs)
        {  
            var range = _options.Check.Range.Split(',').Select(x => x.ToInt()).ToArray();

            if (costs != null && costs.Count() == 3)
            {
                var avg = costs.Average().ToInt();

                if (avg <= range[0])
                {
                    return HealthStatusEnum.IsPassing;
                }
                else if(avg <= range[1] )
                {
                    return HealthStatusEnum.IsWarning;
                }
                else
                {
                    return HealthStatusEnum.IsCritical;
                }
            } 


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var client = _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient);
            client.Timeout = TimeSpan.FromSeconds(range[1]);

            bool HasError = false;

            try
            {
                var response = await client.GetStringAsync(Endpoint);
            }
            catch (Exception ex)
            {
                HasError = true; 
            }
           
            stopwatch.Stop();  

            if (costs != null || HasError || stopwatch.ElapsedMilliseconds.ToInt() > range[0])
            {
                if (costs == null) costs = new List<int>(); 

                costs.Add(HasError ? (range[1] + 1000) : stopwatch.ElapsedMilliseconds.ToInt());

                return await HttpInvoke(Endpoint,costs); 
            }
            else
            {
                return HealthStatusEnum.IsPassing;
            } 
        }
    }
}
