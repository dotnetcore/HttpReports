using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 响应时间分组过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class GroupResponeTimeFilterOption : INodeFilterOption, IStatusCodeFilterOption, ITimeSpanFilterOption
    {
        public string[] Nodes { get; set; }
        public int[] StatusCodes { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// 响应时间分组
        /// </summary>
        public int[,] TimeGroup { get; set; } = DefaultTimeGroup;

        /// <summary>
        /// 默认响应时间分组
        /// </summary>
        public static readonly int[,] DefaultTimeGroup = new[,] { { 0, 100 }, { 100, 500 }, { 500, 1000 }, { 1000, 3000 }, { 3000, 5000 }, { 5000, -1 } };
    }
}