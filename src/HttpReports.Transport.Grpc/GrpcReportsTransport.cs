using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Grpc.Net.Client;
using HttpReports.Collector.Grpc;
using HttpReports.Core.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HttpReports.Transport.Grpc
{
    public class GrpcReportsTransport : IReportsTransport
    {
        public GrpcReportsTransportOptions Options { get; }

        private GrpcCollector.GrpcCollectorClient _client = null;

        private readonly AsyncCallbackDeferFlushCollection<Collector.Grpc.RequestInfoWithDetail> _deferFlushCollection = null;
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

            _deferFlushCollection = new AsyncCallbackDeferFlushCollection<Collector.Grpc.RequestInfoWithDetail>(WriteToRemote, 50, 5);
            _logger = logger;
        }

        private async Task WriteToRemote(List<HttpReports.Collector.Grpc.RequestInfoWithDetail> list, CancellationToken token)
        {
            try
            {
                var pack = new RequestInfoPack();
                var data = list.Select(m => new RequestInfoWithDetail() { Info = m.Info, Detail = m.Detail }).ToArray();
                pack.Data.AddRange(data);

                var reply = await _client.WriteAsync(pack);
            }
            catch (Exception ex)
            {
                //TODO ReTry?
                _logger.LogError(ex, "ReportsTransport Error");
            }
        }

        public Task Write(IRequestInfo requestInfo, IRequestDetail requestDetail)
        {
            _deferFlushCollection.Flush(new RequestInfoWithDetail { 
            
                Info = requestInfo as HttpReports.Collector.Grpc.RequestInfo,
                Detail = requestDetail as HttpReports.Collector.Grpc.RequestDetail 

            });

            return Task.CompletedTask;
        }   

        public async Task WritePerformanceAsync(IPerformance performance)
        {
            try
            {
                var reply = await _client.WritePerformanceAsync(performance as HttpReports.Collector.Grpc.Performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportsTransport Error ");
            }
        }
    }
}