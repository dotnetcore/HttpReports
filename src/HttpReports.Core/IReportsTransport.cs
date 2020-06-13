using HttpReports.Core.Interface;
using System.Threading.Tasks;

namespace HttpReports
{
    public interface IReportsTransport
    {
        Task Write(IRequestInfo requestInfo, IRequestDetail requestDetail);

        Task WritePerformanceAsync(IPerformance performance);

    }
}