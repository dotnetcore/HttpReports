using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 请求信息过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class RequestInfoFilterOption : INodeFilterOption, IStatusCodeFilterOption, ITakeFilterOption, ITimeSpanFilterOption, IOrderFilterOption<RequestInfoFields>
    {
        public string[] Nodes { get; set; }
        public int[] StatusCodes { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public RequestInfoFields Field { get; set; }
        public bool IsAscend { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd";
        public bool IsOrderByField { get; set; } = false;

        public string GetOrderField() => Field.ToString("g");
    }
}