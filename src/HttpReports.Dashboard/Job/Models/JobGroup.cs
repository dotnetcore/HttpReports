using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Job.Models
{

    /// <summary>
    /// 监控组
    /// </summary>
    public class JobGroup
    {
        public int Id { get; set; }

        public string MonitorJson { get; set; }  
    }
}
