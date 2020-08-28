using HttpReports.Core;

using HttpReports.Core.Models;
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

        public Task Transport(RequestBag bag)
        {
            Storage.AddRequestInfoAsync(bag).ConfigureAwait(false);

            return Task.CompletedTask;
        }

        public Task Transport(Performance performance)
        {
            Storage.AddPerformanceAsync(performance);

            return Task.CompletedTask;
        }
    }
}