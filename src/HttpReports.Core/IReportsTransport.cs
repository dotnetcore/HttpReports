using HttpReports.Core;
using HttpReports.Core.Interface;
using System.Threading.Tasks;

namespace HttpReports
{
    public interface IReportsTransport
    {
        Task Transport(RequestBag bag);

        Task Transport(IPerformance performance); 

    }
}