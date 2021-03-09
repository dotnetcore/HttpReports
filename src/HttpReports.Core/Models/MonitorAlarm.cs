 
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class MonitorAlarm
    { 
        public long Id { get; set; } 
        
        public long JobId { get; set; } 

        public string Body { get; set; }

        public DateTime CreateTime { get; set; }

    }
}
