using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options; 
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Transport.Http
{
    public class HttpTransport : IReportsTransport
    {
        public HttpTransportOptions _options { get; } 

        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _deferFlushCollection;
        private readonly ILogger<HttpTransport> _logger;
        private readonly IHttpClientFactory _httpClientFactory; 

        public HttpTransport(IOptions<HttpTransportOptions> options, ILogger<HttpTransport> logger, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value ?? throw new ArgumentNullException();
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _deferFlushCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(WriteToRemote, _options.DeferThreshold,_options.DeferSecond); 
        } 

         private async Task WriteToRemote(List<RequestBag> list, CancellationToken token)
         {
            try
            {
                HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(list),System.Text.Encoding.UTF8, "application/json");  

                var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress,content);  
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            }
        }


        public Task Write(IRequestInfo requestInfo, IRequestDetail requestDetail)
        {
            _deferFlushCollection.Flush(new RequestBag(requestInfo,requestDetail));

            return Task.CompletedTask;
        }

        public async Task WritePerformanceAsync(IPerformance performance)
        { 
            try
            {
                HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(performance as Performance), System.Text.Encoding.UTF8, "application/json"); 

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
