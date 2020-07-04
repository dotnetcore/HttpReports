using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Net.Client;
using HttpReports.Collector.Grpc;
using HttpReports.Core;
using HttpReports.Core.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpReports.Transport.Grpc
{
    public class GrpcReportsTransport : IReportsTransport
    {
        public GrpcReportsTransportOptions Options { get; }

        private GrpcCollector.GrpcCollectorClient _client = null;

        private readonly AsyncCallbackDeferFlushCollection<Collector.Grpc.RequestInfoWithDetail> _RequestCollection;
        private readonly AsyncCallbackDeferFlushCollection<IPerformance> _PerformanceCollection; 

        private readonly ILogger<GrpcReportsTransport> _logger;

        public GrpcReportsTransport(IOptions<GrpcReportsTransportOptions> options, ILogger<GrpcReportsTransport> logger)
        {
            Options = options.Value ?? throw new ArgumentNullException();

            HttpClient httpClient = null;

            if (Options.NeedHttpClientFunc != null)
            {
                httpClient = Options.NeedHttpClientFunc();
            }
            else if (Options.AllowAnyRemoteCertificate)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                httpClient = new HttpClient(httpClientHandler);
            }
            var grpcChannelOptions = new GrpcChannelOptions { HttpClient = httpClient };

            if (Options.PostConfigGrpcChannelOptions != null)
            {
                Options.PostConfigGrpcChannelOptions(grpcChannelOptions);
            }

            var channel = GrpcChannel.ForAddress(Options.CollectorAddress, grpcChannelOptions);
            _client = new GrpcCollector.GrpcCollectorClient(channel);

            _RequestCollection = new AsyncCallbackDeferFlushCollection<RequestInfoWithDetail>(Push, 50, 5);
            _logger = logger;
        }

        private async Task Push(List<Collector.Grpc.RequestInfoWithDetail> list, CancellationToken token)
        {
            try
            {
                var pack = new RequestInfoPack();
                var data = list.Select(m => new RequestInfoWithDetail() { Info = m.Info , Detail = m.Detail }).ToArray();
                pack.Data.AddRange(data);

                var reply = await _client.WriteAsync(pack);
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            }
        }


        private async Task Push(List<IPerformance> list, CancellationToken token)
        {
            try
            {
                var reply = await _client.WritePerformanceAsync(null);
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            }
        }


        public Task Transport(RequestBag bag)
        {
            _RequestCollection.Flush(new RequestInfoWithDetail { 
            
                Info = bag.RequestInfo as Collector.Grpc.RequestInfo,
                Detail= bag.RequestDetail as Collector.Grpc.RequestDetail 
            
            });

            return Task.CompletedTask;
        }

        public Task Transport(IPerformance performance)
        {
            _PerformanceCollection.Flush(performance);

            return Task.CompletedTask;
        }
    }
}