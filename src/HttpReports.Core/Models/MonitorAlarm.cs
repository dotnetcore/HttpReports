using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class MonitorAlarm
    { 
        public string Id { get; set; }

        public string JobId { get; set; } 

        public string Body { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
