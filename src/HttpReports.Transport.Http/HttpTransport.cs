using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Transport.Http
{
    public class HttpTransport : IReportsTransport
    {
        public HttpTransportOptions _options { get; } 

        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _RequestBagCollection; 

        private readonly ILogger<HttpTransport> _logger;
        private readonly IHttpClientFactory _httpClientFactory; 

        public HttpTransport(IOptions<HttpTransportOptions> options, ILogger<HttpTransport> logger, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value ?? throw new ArgumentNullException();
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _RequestBagCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(Push, _options.DeferThreshold,_options.DeferSecond); 
        }   

        public Task Transport(RequestBag bag)
        {
            _RequestBagCollection.Flush(bag);

            return Task.CompletedTask;
        }

        public async Task Transport(IPerformance performance)
        { 
            try
            {
                HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(performance), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress, content);
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            } 
        }
        

        private async Task Push(List<RequestBag> list, CancellationToken token)
        {
            try
            {
                HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(list), System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress, content);
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            }
        } 
          
    }
}
