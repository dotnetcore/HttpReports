using Grpc.Net.Client;
using HttpReports.Collector.Grpc;
using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Performance = HttpReports.Core.Models.Performance;

namespace HttpReports.Transport.Grpc
{
    public class GrpcTransport : IReportsTransport
    { 
        private readonly GrpcCollector.GrpcCollectorClient _client;
        private readonly GrpcTransportOptions _options;
        private readonly ILogger<GrpcTransport> _logger;
        private readonly AsyncCallbackDeferFlushCollection<RequestBag> _RequestBagCollection;

        public GrpcTransport(IOptions<GrpcTransportOptions> options, ILogger<GrpcTransport> logger)
        {
            _options = options.Value;
            _logger = logger;  
            _client = new GrpcCollector.GrpcCollectorClient(GrpcChannel.ForAddress(_options.CollectorAddress)); 
            _RequestBagCollection = new AsyncCallbackDeferFlushCollection<RequestBag>(Push, _options.DeferThreshold, _options.DeferSecond);
        }


        public Task SendDataAsync(RequestBag bag)
        {
            _RequestBagCollection.Flush(bag);

            return Task.CompletedTask;
        }

        public async Task SendDataAsync(Performance performance)
        {
            try
            {
                var model = JsonSerializer.Deserialize<HttpReports.Collector.Grpc.Performance>(JsonSerializer.Serialize(performance));

                if (model != null)
                {
                    var reply = await _client.WritePerformanceAsync(model);
                }  
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpReports transport failed:{ex.Message}"); 
            }
        }

        private async Task Push(List<RequestBag> list, CancellationToken token)
        {
            try
            {  
                var pack = JsonSerializer.Deserialize<RequestInfoPack>(JsonSerializer.Serialize(list));

                if (pack != null)
                {
                    var reply = await _client.WriteRequestAsync(pack);
                } 
                
            }
            catch (Exception ex)
            { 
                _logger.LogWarning($"HttpReports transport failed:{ex.Message}");

            }
        } 

    }
}
