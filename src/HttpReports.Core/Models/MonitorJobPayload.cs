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

        public int TimeoutMs { get; set; }

        public double Percentage { get; set; }
    }

    public class ErrorMonitorRule
    {
        public int Status { get; set; }

        public double Percentage { get; set; }

        public int ErrorCount { get; set; }
    }


    public class CallMonitorRule
    {
        public int Status { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

    }
}
