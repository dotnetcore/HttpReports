using HttpReports.Core; 
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text; 
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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

        public Task SendDataAsync(RequestBag bag)
        { 
            _RequestBagCollection.Flush(bag);

            return Task.CompletedTask;
        }

        public async Task SendDataAsync(Performance performance)
        {
            await Retry(async () => {

                try
                {
                    HttpContent content = new StringContent(HttpUtility.HtmlEncode(JsonConvert.SerializeObject(performance)), System.Text.Encoding.UTF8, "application/json");

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    content.Headers.Add(BasicConfig.TransportType, typeof(Performance).Name);

                    var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress + BasicConfig.TransportPath.Substring(1), content);

                    return response.StatusCode == HttpStatusCode.OK;
                } 
                catch(Exception ex)
                {
                    //_logger.LogError(ex, "performance push failed:" + ex.ToString());
                    return false;
                }   
            });
        }
        

        private async Task Push(List<RequestBag> list, CancellationToken token)
        { 
           await Retry(async () => {

               try
               {
                   HttpContent content = new StringContent(HttpUtility.HtmlEncode(JsonConvert.SerializeObject(list)), System.Text.Encoding.UTF8);

                   content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                   content.Headers.Add(BasicConfig.TransportType, typeof(RequestBag).Name);
                   var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress + BasicConfig.TransportPath.Substring(1), content);

                   var result = await response.Content.ReadAsStringAsync();

                   return response.StatusCode == HttpStatusCode.OK;

               } 
               catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException) 
               {
                   _logger.LogWarning("HttpReports push failed:Network Error...");
                   return false;
               }
               catch (Exception ex)
               { 
                   _logger.LogWarning(ex, "HttpReports push failed:" + ex.ToString());
                   return false;
               } 

            });   
        }

        private async Task<bool> Retry(Func<Task<bool>> func, int retry = 3)
        {    
            for (int i = 0; i < 3; i++)
            {
                if (await func.Invoke())
                {
                    return true;
                }  
            }

            return false;
        }  

    }
}
