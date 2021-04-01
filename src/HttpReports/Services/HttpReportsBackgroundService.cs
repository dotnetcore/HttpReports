using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Services
{
    public class HttpReportsBackgroundService : IHostedService
    {
        private ILogger<HttpReportsBackgroundService> _logger { get; }
        private IReportsTransport _transport { get; }

        private IPerformanceService _performanceService;

        private IConfiguration _config;

        private HttpReportsOptions _options;

        public HttpReportsBackgroundService(IOptions<HttpReportsOptions> options, IConfiguration configuration, ILogger<HttpReportsBackgroundService> logger,IReportsTransport reportsTransport, IPerformanceService performanceService)
        {
            _logger = logger;
            _performanceService = performanceService;
            _transport = reportsTransport;
            _config = configuration;
            _options = options?.Value;
        }

        public Task StartAsync(CancellationToken token = default)
        {
            try
            {
                _ = Task.Run(async () => { 
                     
                    while (!token.IsCancellationRequested)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(10), token);

                        Uri uri = new Uri(_options.Server);

                        Performance performance = await _performanceService.GetPerformance(uri.Host + ":" + uri.Port);

                        if (performance != null)
                        {
                            await _transport.SendDataAsync(performance);
                        } 
                       
                    } 

                }); 
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"HttpReports BackgroundService Error,ex:{ex.ToString()}");
            }

            return Task.CompletedTask;
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

    }
}
