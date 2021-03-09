using HttpReports.Core;
using HttpReports.Core.Models;
using System.Threading.Tasks;

namespace HttpReports
{
    public interface IReportsTransport
    {
        Task SendDataAsync(RequestBag bag);

        Task SendDataAsync(Performance performance); 

    }
}