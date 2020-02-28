using System.Collections.Generic;

using HttpReports.Storage.FilterOptions;

namespace HttpReports.Models
{
    /// <summary>
    /// 响应时间统计结果
    /// </summary>
    public class ResponseTimeStatisticsResult
    {
        /// <summary>
        /// 基于指定统计类型的列表
        /// </summary>
        public Dictionary<string, int> Items { get; set; }

        /// <summary>
        /// 统计类型
        /// </summary>
        public TimeUnit Type { get; set; }
    }
}