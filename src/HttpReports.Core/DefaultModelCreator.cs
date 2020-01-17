using System;

using HttpReports.Monitor;

namespace HttpReports
{
    public class DefaultModelCreator : IModelCreator
    {
        public virtual IMonitor CreateMonitor(MonitorType type)
        {
            switch (type)
            {
                case MonitorType.ResponseTimeOut:
                    return new ResponseTimeOutMonitor();

                case MonitorType.ErrorResponse:
                    return new ErrorResponseMonitor();

                case MonitorType.ToManyRequestWithAddress:
                    return new RequestTimesMonitor();

                case MonitorType.ToManyRequestBySingleRemoteAddress:
                    return new RemoteAddressRequestTimesMonitor();

                case MonitorType.UnDefine:
                default:
                    throw new UnSupportMonitorException($"Unknown Type:{type}");
            }
        }

        public virtual IMonitorRule CreateMonitorRule() => new MonitorRule();

        public virtual IRequestInfo CreateRequestInfo() => new RequestInfo();
    }
}