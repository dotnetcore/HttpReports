using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.ViewModels
{
    public class AddMonitorRuleRequest
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Email { get; set; }

        public ResponseTimeOutMonitorViewModel ResponseTimeOutMonitor { get; set; }

        public ErrorResponseMonitorViewModel ErrorResponseMonitor { get; set; }

        public RemoteAddressRequestTimesMonitorViewModel RemoteAddressRequestTimesMonitor { get; set; }

        public RequestTimesMonitorViewModel RequestTimesMonitor { get; set; } 

    }


    /// <summary>
    /// 响应超时监控
    /// </summary>
    public class ResponseTimeOutMonitorViewModel
    {
        public int Id { get; set; }

        public int Interval { get; set; }

        public string Percentage { get; set; }

        public string TimeoutThreshold { get; set; }

    } 

    /// <summary>
    /// 请求错误监控
    /// </summary>
    public class ErrorResponseMonitorViewModel
    {
        public int Id { get; set; }

        public int Interval { get; set; }

        public string StatusCodes { get; set; }  

        public string Percentage { get; set; }  

    }   


    /// <summary>
    /// IP监控
    /// </summary>
    public class RemoteAddressRequestTimesMonitorViewModel
    { 
        public int Id { get; set; } 

        public int Interval { get; set; }

        public string Percentage { get; set; }

        public string WhileList { get; set; }

    }


    /// <summary>
    /// 请求量过多监控
    /// </summary>
    public class RequestTimesMonitorViewModel
    {
        public int Id { get; set; }

        public int Interval { get; set; }

        public string WarningThreshold { get; set; }

    } 

}
