using System;
using System.Runtime.Serialization;

namespace HttpReports
{
    /// <summary>
    /// 不支持的监控类型异常
    /// </summary>
    public class UnSupportMonitorException : HttpReportsException
    {
        public UnSupportMonitorException()
        {
        }

        public UnSupportMonitorException(string message) : base(message)
        {
        }

        public UnSupportMonitorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnSupportMonitorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}