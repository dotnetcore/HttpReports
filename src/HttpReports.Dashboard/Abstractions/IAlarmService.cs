using HttpReports.Dashboard.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Abstractions
{
    public interface IAlarmService
    {
        Task AlarmAsync(AlarmOption option);

    }
}
