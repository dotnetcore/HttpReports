using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Job
{
    public  class BaseMonitor
    { 
        public string MonitorType { get; set; }

        public int State { get; set; }

        public string Percent { get; set; }

        public string Max { get; set; }

        public string Min { get; set; }

        public string HttpCodeList { get; set; }

        public string IPWhiteList { get; set; }

        public string ResponseTime { get; set; } 
       
    }
}
