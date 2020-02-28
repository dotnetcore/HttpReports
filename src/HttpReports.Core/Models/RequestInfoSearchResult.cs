using System.Collections.Generic;

using HttpReports.Storage.FilterOptions;

namespace HttpReports.Models
{
    /// <summary>
    /// 请求信息搜索结果
    /// </summary>
    public class RequestInfoSearchResult
    {
        /// <summary>
        /// 请求信息列表
        /// </summary>
        public List<IRequestInfo> List { get; set; } = new List<IRequestInfo>();

        /// <summary>
        /// 请求的总条数
        /// </summary>
        public int AllItemCount { get; set; }

        /// <summary>
        /// 使用的搜索选项
        /// </summary>
        public RequestInfoSearchFilterOption SearchOption { get; set; }
    }
}