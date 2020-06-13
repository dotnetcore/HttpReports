using HttpReports.Core.Interface;

namespace HttpReports.Transport.Grpc
{
    internal class ModelCreator : DefaultModelCreator
    {
        public override IRequestInfo CreateRequestInfo() => new Collector.Grpc.RequestInfo();

        public override IRequestDetail CreateRequestDetail() => new Collector.Grpc.RequestDetail();

        public override IPerformance CreatePerformance() => new Collector.Grpc.Performance();

    }
}