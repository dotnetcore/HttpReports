using System;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 基于时间跨度的过滤选项
    /// </summary>
    public interface ITimeSpanFilterOption : IFilterOption
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        DateTime? StartTime { get; set; }

        /// <summary>
        /// 起始时间格式
        /// </summary>
        string StartTimeFormat { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        DateTime? EndTime { get; set; }

        /// <summary>
        /// 结束时间格式
        /// </summary>
        string EndTimeFormat { get; set; }
    }
}