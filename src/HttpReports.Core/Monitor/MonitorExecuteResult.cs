using System.Collections.Generic;

namespace HttpReports.Monitor
{
    /// <summary>
    /// 监控检查结果
    /// </summary>
    public class MonitorExecuteResult
    {
        /// <summary>
        /// 执行是否成功
        /// </summary>
        public bool ExecuteSuccess { get; set; }

        /// <summary>
        /// 是否通过了检查
        /// </summary>
        public bool PassThrough { get; set; }
    }
}