using HttpReports.Core.Models;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Abstractions
{
    public interface IScheduleService
    {
        Task InitAsync(); 

        Task InitMonitorJobAsync();

        Task ScheduleJobAsync(MonitorJob model);


        Task DeleteJobAsync(IJobDetail job);


        Task UpdateMonitorJobAsync(MonitorJob deleteJob = null);

        Task AutoClearDataJobAsync();
         
    }
}
