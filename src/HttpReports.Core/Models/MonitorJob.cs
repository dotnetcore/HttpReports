using HttpReports.Monitor;
using System;
using System.Collections.Generic; 
using System.Text;

namespace HttpReports.Models
{
    public class MonitorJob: IMonitorJob
    {
        public string Id { get; set; } 
         
        public string Title { get; set; }

        public string Description { get; set; }

        public string CronLike { get; set; }

        public string WebHook { get; set; }

        public string Emails { get; set; }

        public string Mobiles { get; set; }

        public int Status { get; set; }

        public string Nodes { get; set; }

        public string Payload { get; set; }

        public DateTime CreateTime { get; set; }

    }

    public class MonitorJobPayload
    {
        public ResponseTimeOutMonitorJob ResponseTimeOutMonitor { get; set; }

        public ErrorResponseMonitorJob ErrorResponseMonitor { get; set; }

        public IPMonitorJob IPMonitor { get; set; }

        public RequestCountMonitorJob RequestCountMonitor { get; set; }

    }


    public class ResponseTimeOutMonitorJob
    {
        public int Status { get; set; }

        public int TimeOutMs { get; set; }

        public double Percentage { get; set; }
    }

    public class ErrorResponseMonitorJob
    {
        public int Status { get; set; }

        public string HttpCodeStatus { get; set; }

        public double Percentage { get; set; }

    }

    public class IPMonitorJob
    {
        public int Status { get; set; }

        public string WhiteList { get; set; }

        public double Percentage { get; set; }

    }

    public class RequestCountMonitorJob
    {
        public int Status { get; set; }

        public int Max { get; set; }

    }
}
