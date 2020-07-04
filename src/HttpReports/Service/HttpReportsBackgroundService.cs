using HttpReports.Core.Config;
using HttpReports.Core.Interface;
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

namespace HttpReports.Service
{
    public class HttpReportsBackgroundService : IBackgroundService
    { 
        private ILogger<HttpReportsBackgroundService> _logger { get; } 
        private IReportsTransport _transport { get; }

        private IPerformanceService _performanceService;

        private IConfiguration _config;

        private HttpReportsOptions _options;

        
        public HttpReportsBackgroundService(IOptions<HttpReportsOptions> options, IConfiguration configuration,ILogger<HttpReportsBackgroundService> logger,IHttpContextAccessor contextAccessor, IReportsTransport reportsTransport, IPerformanceService performanceService)
        { 
            _logger = logger;
            _performanceService = performanceService;
            _transport = reportsTransport;
            _config = configuration;
            _options = options?.Value;
        }

        public async Task StartAsync(IApplicationBuilder builder, CancellationToken Token = default)
        {  
            try
            {  
                _logger.LogInformation($"HttpReports BackgroundService Start...");

                await ExecuteAsync();
            }
            catch (Exception ex)
            { 
                _logger.LogError($"HttpReports BackgroundService Error,ex:{ex.ToString()}");
            } 
        }

        public async Task ExecuteAsync(CancellationToken Token = default)
        {  
            while (!Token.IsCancellationRequested)
            {
                Uri uri = new Uri(_options.Urls);

                _logger.LogInformation($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} BackgroundService Execute... "); 

                IPerformance performance = await _performanceService.GetPerformance(uri.Host+":"+uri.Port);

                if (performance != null)
                {
                    await _transport.Transport(performance);
                }

                await Task.Delay(TimeSpan.FromSeconds(10), Token); 
            }  
        } 

    }
}
