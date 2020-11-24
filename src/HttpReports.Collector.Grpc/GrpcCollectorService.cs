using System.Threading.Tasks; 
using Grpc.Core;
using HttpReports.Core;
using HttpReports.Storage.Abstractions;

namespace HttpReports.Collector.Grpc
{
    public class GrpcCollectorService : GrpcCollector.GrpcCollectorBase
    {
        public readonly IHttpReportsStorage _storage;

        public GrpcCollectorService(IHttpReportsStorage storage)
        {
            _storage = storage;
        } 

        public override async Task<WriteReply> WritePerformance(Performance performance, ServerCallContext context)
        {
            Core.Models.Performance model = new Core.Models.Performance() {
            
                Id = performance.Id,
                CreateTime = new System.DateTime(performance.CreateTimeStamp),
                GCGen0 = performance.GCGen0,
                GCGen1 = performance.GCGen1,
                GCGen2 = performance.GCGen2,
                HeapMemory = performance.HeapMemory,
                Service = performance.Service,
                Instance = performance.Instance,
                PendingThreadCount = performance.PendingThreadCount,
                ProcessCPU = performance.ProcessCPU,
                ProcessMemory = performance.ProcessMemory,
                ThreadCount = performance.ThreadCount 
            }; 

            await _storage.AddPerformanceAsync(model);

            return new WriteReply() { Code = 0 }; 
        }


        public override async Task<WriteReply> WriteRequest(RequestInfoPack request, ServerCallContext context)
        {
            foreach (var item in request.Data)
            {
                var info = new Core.Models.RequestInfo {
                
                     Id = item.RequestInfo.Id,
                     Service = item.RequestInfo.Service,
                     Instance = item.RequestInfo.Instance,
                     CreateTime = new System.DateTime(item.RequestInfo.CreateTimeStamp),
                     LoginUser = item.RequestInfo.LoginUser,
                     Method = item.RequestInfo.Method,
                     Milliseconds = item.RequestInfo.Milliseconds,
                     ParentId = item.RequestInfo.ParentId,
                     ParentService = item.RequestInfo.ParentService,
                     RemoteIP = item.RequestInfo.RemoteIP,
                     RequestType = item.RequestInfo.RequestType,
                     Route = item.RequestInfo.Route,
                     StatusCode = item.RequestInfo.StatusCode,
                     Url = item.RequestInfo.Url 
                };

                var detail = new Core.Models.RequestDetail {

                    Id = item.RequestDetail.Id,
                    RequestId = item.RequestDetail.RequestId,
                    Cookie = item.RequestDetail.Cookie,
                    CreateTime = new System.DateTime(item.RequestDetail.CreateTimeStamp),
                    ErrorMessage = item.RequestDetail.ErrorMessage,
                    ErrorStack = item.RequestDetail.ErrorStack,
                    Header = item.RequestDetail.Header,
                    QueryString = item.RequestDetail.QueryString,
                    RequestBody = item.RequestDetail.RequestBody,
                    ResponseBody = item.RequestDetail.ResponseBody,
                    Scheme = item.RequestDetail.Scheme 
                }; 

                RequestBag bag = new RequestBag(info,detail);

                await _storage.AddRequestInfoAsync(bag);
            }

            return new WriteReply() { Code = 0 };
        }
    }
}