using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class MonitorJobPayload
    {
        public ResponseTimeMonitorRule ResponseTimeMonitor { get; set; }

        public ErrorMonitorRule ErrorMonitor { get; set; }

        public CallMonitorRule CallMonitor { get; set; }

    }


    public class ResponseTimeMonitorRule
    {
        public int Status { get; set; }

        public int Timeout { get; set; }

        public int Percentage { get; set; }
    }

    public class ErrorMonitorRule
    {
        public int Status { get; set; }

        public int Percentage { get; set; } 
    }


    public class CallMonitorRule
    {
        public int Status { get; set; } 

        public int Min { get; set; }

        public int Max { get; set; }

    }
}
