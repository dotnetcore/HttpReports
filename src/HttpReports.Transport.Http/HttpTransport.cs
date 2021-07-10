using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
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
        private readonly JsonSerializerOptions _jsonSetting;


        public HttpTransport(IOptions<HttpTransportOptions> options, JsonSerializerOptions jsonSetting, ILogger<HttpTransport> logger, IHttpClientFactory httpClientFactory)
        {
            _options = options.Value ?? throw new ArgumentNullException();
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _jsonSetting = jsonSetting;
            if (_options.CollectorAddress != null)
                _RequestBagCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(Push, _options.DeferThreshold, _options.DeferSecond);
        }

        public Task SendDataAsync(RequestBag bag)
        {
            if (_RequestBagCollection != null)
                _RequestBagCollection.Flush(bag);

            return Task.CompletedTask;
        }

        public async Task SendDataAsync(Performance performance)
        {
            if (_options.CollectorAddress == null) return;
            await Retry(async () =>
            {

                try
                {
                    HttpContent content = new StringContent(HttpUtility.HtmlEncode(System.Text.Json.JsonSerializer.Serialize(performance, _jsonSetting)), System.Text.Encoding.UTF8, "application/json");

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    content.Headers.Add(BasicConfig.TransportType, typeof(Performance).Name);

                    var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress + BasicConfig.HttpCollectorEndpoint.Substring(1), content);

                    return response.StatusCode == HttpStatusCode.OK;
                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                {
                    _logger.LogWarning("HttpReports transport failed...");
                    return false;
                }
                catch (Exception ex)
                {
                    //_logger.LogError(ex, "performance push failed:" + ex.ToString());
                    return false;
                }

            });

        }

        private async Task Push(List<RequestBag> list, CancellationToken token)
        {
            if (_options.CollectorAddress == null) return;
            await Retry(async () =>
            {

                try
                {
                    HttpContent content = new StringContent(HttpUtility.HtmlEncode(System.Text.Json.JsonSerializer.Serialize(list, _jsonSetting)), System.Text.Encoding.UTF8);

                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    content.Headers.Add(BasicConfig.TransportType, typeof(RequestBag).Name);
                    var response = await _httpClientFactory.CreateClient(BasicConfig.HttpReportsHttpClient).PostAsync(_options.CollectorAddress + BasicConfig.HttpCollectorEndpoint.Substring(1), content);

                    var result = await response.Content.ReadAsStringAsync();

                    return response.StatusCode == HttpStatusCode.OK;

                }
                catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
                {
                    _logger.LogWarning("HttpReports transport failed...");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("HttpReports transport failed:" + ex.ToString());
                    return false;
                }

            });
        }

        private async Task<bool> Retry(Func<Task<bool>> func, int retry = 3)
        {
            for (int i = 0; i < 3; i++)
            {
                await Task.Delay(200);

                if (await func.Invoke())
                {
                    return true;
                }
            }

            return false;
        }

    }
}
