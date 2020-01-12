using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 请求信息过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class IndexPageDataFilterOption : INodeFilterOption, ITimeSpanFilterOption
    {
        public string[] Nodes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd";
    }
}