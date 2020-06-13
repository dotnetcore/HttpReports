using HttpReports.Core.Interface;
using System.Threading.Tasks;

namespace HttpReports
{
    public class DirectlyReportsTransport : IReportsTransport
    {
        public IHttpReportsStorage Storage { get; }

        public DirectlyReportsTransport(IHttpReportsStorage storage)
        {
            Storage = storage;
        }  

        public Task Write(IRequestInfo requestInfo, IRequestDetail requestDetail)
        {
            Storage.AddRequestInfoAsync(requestInfo, requestDetail).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task WritePerformanceAsync(IPerformance performance)
        {
            Storage.AddPerformanceAsync(performance);

            return Task.CompletedTask;
        }
    }
}