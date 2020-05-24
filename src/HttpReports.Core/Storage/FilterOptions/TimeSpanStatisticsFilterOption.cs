using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 基于时间段的请求统计过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class TimeSpanStatisticsFilterOption : INodeFilterOption, IStatusCodeFilterOption, ITimeSpanFilterOption
    {
        public string[] Nodes { get; set; }
        public int[] StatusCodes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// 统计类型
        /// </summary>
        public TimeUnit Type { get; set; } = TimeUnit.Day;
        public string Service { get; set; }
        public string LocalIP { get; set; }
        public int LocalPort { get; set; }
    }
}