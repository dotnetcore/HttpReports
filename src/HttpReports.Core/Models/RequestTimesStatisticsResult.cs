using System.Collections.Generic;

using HttpReports.Storage.FilterOptions;

namespace HttpReports.Models
{
    /// <summary>
    /// 请求次数统计结果
    /// </summary>
    public class RequestTimesStatisticsResult
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