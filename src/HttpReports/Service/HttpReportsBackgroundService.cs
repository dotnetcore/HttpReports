using HttpReports.Core.Config;
using HttpReports.Core.Interface;
using HttpReports.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Service
{
    public class HttpReportsBackgroundService : IBackgroundService
    { 
        private ILogger<HttpReportsBackgroundService> _logger { get; }
       
        private IHttpReportsStorage _storage;

        private ServiceInstance serviceInstance;

        private object locker = new object();

        private IPerformanceService _performanceService;

        
        public HttpReportsBackgroundService(ILogger<HttpReportsBackgroundService> logger,IHttpContextAccessor contextAccessor,IHttpReportsStorage storage, IPerformanceService performanceService)
        { 
            _logger = logger;
            _performanceService = performanceService;
            _storage = storage;
        }

        public async Task StartAsync(IApplicationBuilder builder, CancellationToken Token = default)
        {
            try
            {
                var localhost = builder.ServerFeatures?.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();

                _logger.LogInformation("HttpReports BackgroundService Start...");

                await ExecuteAsync(localhost);
            }
            catch (Exception ex)
            { 
                _logger.LogError($"HttpReports BackgroundService Error,ex:{ex.ToString()}");
            } 
        }

        public async Task ExecuteAsync(string localhost,CancellationToken Token = default)
        {  
            while (!Token.IsCancellationRequested)
            {
                if (serviceInstance == null)
                {
                    if (!localhost.IsEmpty()) 
                       await new HttpClient().GetStringAsync($"{localhost}{BasicConfig.HttpReportsServerRegister}");

                    await Task.Delay(TimeSpan.FromSeconds(1), Token);
                }
                else
                {
                    _logger.LogInformation($"HttpReportsBackgroundService Execute IP:{serviceInstance.LocalIP} Port:{serviceInstance.LocalPort}"); 

                    IPerformance performance = await _performanceService.GetPerformance(serviceInstance.LocalIP+":"+serviceInstance.LocalPort);

                    if (performance != null)
                    {
                        await _storage.AddPerformanceAsync(performance);
                    }

                    await Task.Delay(TimeSpan.FromSeconds(10), Token);
                } 
            }  
        }

        public void MapBackgroundService(IApplicationBuilder builder)
        {
             builder.Map(BasicConfig.HttpReportsServerRegister, _ => {

                _.Run(async context => {
 
                    lock (locker)
                    {
                        serviceInstance = new ServiceInstance {
                        
                            LocalIP = context.Connection.LocalIpAddress.ToString(),
                            LocalPort = context.Connection.LocalPort 

                        };

                        if (serviceInstance.LocalIP == "::1") serviceInstance.LocalIP = "127.0.0.1";
                    } 

                    await Task.CompletedTask;

                });

            }); 
            
        } 
    }
}
