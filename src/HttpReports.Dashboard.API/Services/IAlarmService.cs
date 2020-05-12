using System.Threading.Tasks;

using HttpReports.Dashboard.Models;

namespace HttpReports.Dashboard.Services
{
    /// <summary>
    /// 告警服务
    /// </summary>
    public interface IAlarmService
    {
        Task AlarmAsync(AlarmOption option);
    }
}