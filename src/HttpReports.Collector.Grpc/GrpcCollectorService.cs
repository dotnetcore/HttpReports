using System.Threading.Tasks;

using Grpc.Core;

namespace HttpReports.Collector.Grpc
{
    public class GrpcCollectorService : GrpcCollector.GrpcCollectorBase
    {
        public IHttpReportsStorage Storage { get; }

        public GrpcCollectorService(IHttpReportsStorage storage)
        {
            Storage = storage;
        }

        public override async Task<WriteReply> Write(RequestInfoPack request, ServerCallContext context)
        {
            foreach (var item in request.Data)
            {
                await Storage.AddRequestInfoAsync(item.Info, item.Detail);
            }

            return new WriteReply() { Code = 0 };
        }
    }
}