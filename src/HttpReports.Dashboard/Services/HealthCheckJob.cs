using HttpReports.Core;
using HttpReports.Core.ViewModels;
using HttpReports.Dashboard.Abstractions;
using HttpReports.Dashboard.Implements;
using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        private ILogger<HealthCheckJob> _logger;

        public async Task Execute(IJobExecutionContext context)
        {
            _storage = _storage ?? ServiceContainer.provider.GetService(typeof(IHttpReportsStorage)) as IHttpReportsStorage;
            _options = _options ?? (ServiceContainer.provider.GetService(typeof(IOptions<DashboardOptions>)) as IOptions<DashboardOptions>).Value;
            _healthCheckService = ServiceContainer.provider.GetService(typeof(IHealthCheckService)) as IHealthCheckService;
            _httpClientFactory = ServiceContainer.provider.GetService(typeof(IHttpClientFactory)) as IHttpClientFactory;

            _logger = _logger ?? ServiceContainer.provider.GetService(typeof(ILogger<HealthCheckJob>)) as ILogger<HealthCheckJob>;

            await CheckAsync();
        }

        public async Task CheckAsync()
        {    
            var list = await _healthCheckService.GetServiceInstance(); 

            foreach (var item in list)
            {    
                int Passing = 0;
                int Warning = 0;
                int Critical = 0; 

                foreach (var k in item.Instances)
                {
                    k.Status = HealthStatusEnum.None;

                    if (_options.Check.Mode.ToUpperInvariant() == "SELF")
                    {
                        k.Status = await SelfInvoke(item.ServiceInfo.Service,k.Instance);
                    }

                    if (_options.Check.Mode.ToUpperInvariant() == "HTTP")
                    {
                        k.Status = await HttpInvoke(k.Instance + _options.Check.Endpoint, null);
                    }  

                    if (k.Status == HealthStatusEnum.IsPassing)
                    {
                        Passing += 1; 
                    }

                    if (k.Status == HealthStatusEnum.IsWarning)
                    {
                        Warning += 1;
                    }

                    if (k.Status == HealthStatusEnum.IsCritical)
                    {
                        Critical += 1;
                    } 
                }

                item.ServiceInfo.Passing = Passing;
                item.ServiceInfo.Warning = Warning;
                item.ServiceInfo.Critical = Critical;

            } 
        } 

        public async Task<HealthStatusEnum> HttpInvoke(string Endpoint, List<int> costs)
        {
            if (!Endpoint.Contains("http"))
            {
                Endpoint = "http://" + Endpoint;
            } 

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

            bool HasError = false;

            try
            {
                client.Timeout = TimeSpan.FromMilliseconds(range[1].ToString().ToInt());
                var response = await client.GetStringAsync(Endpoint);
            }
            catch (Exception ex)
            {
                //_logger.LogError($"Health Check Error:{ex.ToString()}");

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

        public async Task<HealthStatusEnum> SelfInvoke(string Service,string Instance)
        {
            bool performance =  await _storage.GetPerformanceAsync(DateTime.Now.AddSeconds(-30),DateTime.Now,Service,Instance);

            return performance ? HealthStatusEnum.IsPassing : HealthStatusEnum.IsCritical;
        
        }  
    }
}
