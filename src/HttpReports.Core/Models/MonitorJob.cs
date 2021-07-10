 
using System;
using System.Collections.Generic; 
using System.Text;

namespace HttpReports.Core.Models
{
    public class MonitorJob 
    { 
        public long Id { get; set; } 
         
        public string Title { get; set; }

        public string Description { get; set; }

        public string CronLike { get; set; }

        public string WebHook { get; set; }

        public string Emails { get; set; }

        public string Mobiles { get; set; }

        public int Status { get; set; } 

        public string Service { get; set; } 

        public string Instance { get; set; }

        public string Payload { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public DateTime CreateTime { get; set; }

    } 
   
}
