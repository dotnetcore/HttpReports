using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Job.Models
{ 
    /// <summary>
    /// 监控任务
    /// </summary>
    public class JobTask
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public int GroupId { get; set; }  

    }
}
