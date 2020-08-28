using HttpReports.Core;
using HttpReports.Core.Models;
using System.Threading.Tasks;

namespace HttpReports
{
    public interface IReportsTransport
    {
        Task Transport(RequestBag bag);

        Task Transport(Performance performance); 

    }
}