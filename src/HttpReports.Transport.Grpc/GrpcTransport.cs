using Grpc.Core;
using Grpc.Net.Client;
using HttpReports.Collector.Grpc;
using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            var httpClient = new HttpClient(httpClientHandler);

            GrpcChannelOptions grpcChannelOptions = new GrpcChannelOptions() {  HttpClient = httpClient };

            _client = new GrpcCollector.GrpcCollectorClient(GrpcChannel.ForAddress(_options.CollectorAddress, grpcChannelOptions));  

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
            catch (Exception ex) when (ex is RpcException) 
            {
                _logger.LogWarning($"HttpReports transport failed"); 
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
                var pack = new RequestInfoPack();

                foreach (var x in list)
                {
                    var value = new RequestInfoWithDetail();

                    value.RequestInfo = new Collector.Grpc.RequestInfo {
                    
                        Id = x.RequestInfo.Id,
                        ParentId = x.RequestInfo.ParentId,
                        Service = x.RequestInfo.Service,
                        ParentService = x.RequestInfo.ParentService,
                        Instance = x.RequestInfo.Instance,
                        Route = x.RequestInfo.Route,
                        Url = x.RequestInfo.Url,
                        RequestType = x.RequestInfo.RequestType,
                        Method = x.RequestInfo.Method,
                        Milliseconds = x.RequestInfo.Milliseconds,
                        StatusCode = x.RequestInfo.StatusCode,
                        RemoteIP = x.RequestInfo.RemoteIP,
                        LoginUser = x.RequestInfo.LoginUser,
                        CreateTime = x.RequestInfo.CreateTime,
                        CreateTimeStamp = x.RequestInfo.CreateTime.Ticks
                    };

                    value.RequestDetail = new Collector.Grpc.RequestDetail { 
                    
                        Id = x.RequestDetail.Id,
                        RequestId = x.RequestDetail.RequestId,
                        Scheme = x.RequestDetail.Scheme,
                        QueryString = x.RequestDetail.QueryString,
                        Header = x.RequestDetail.Header,
                        Cookie = x.RequestDetail.Cookie,
                        RequestBody = x.RequestDetail.RequestBody,
                        ResponseBody = x.RequestDetail.ResponseBody,
                        ErrorMessage = x.RequestDetail.ErrorMessage,
                        ErrorStack = x.RequestDetail.ErrorStack,
                        CreateTime = x.RequestDetail.CreateTime,
                        CreateTimeStamp = x.RequestDetail.CreateTime.Ticks 
                    };

                    pack.Data.Add(value);
                }   

                var reply = await _client.WriteRequestAsync(pack); 
            }
            catch (Exception ex) when (ex is RpcException)
            {
                _logger.LogWarning($"HttpReports transport failed");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"HttpReports transport failed:{ex.Message}");
            }
        } 

    }
}
