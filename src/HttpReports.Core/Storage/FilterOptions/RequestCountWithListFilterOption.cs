using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 有白名单的请求统计过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class RequestCountWithListFilterOption : RequestCountFilterOption
    {
        /// <summary>
        /// 白名单
        /// </summary>
        public string[] List { get; set; }

        /// <summary>
        /// 获取列表内的数据
        /// </summary>
        public bool InList { get; set; } = false;
    }
}