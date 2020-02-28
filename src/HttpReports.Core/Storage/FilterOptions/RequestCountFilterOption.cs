using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 请求统计过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class RequestCountFilterOption : INodeFilterOption, IStatusCodeFilterOption, ITakeFilterOption, ITimeSpanFilterOption
    {
        private int _page;
        private int _pageSize;
        public string[] Nodes { get; set; }
        public int[] StatusCodes { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";

        public int Page
        {
            get => _page;
            set
            {
                _page = value;
                ReSetTake();
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value;
                ReSetTake();
            }
        }

        /// <summary>
        /// 重新设置take的信息
        /// </summary>
        private void ReSetTake()
        {
            if (_pageSize > 0 && _page > 0)
            {
                Skip = (_page - 1) * PageSize;
            }
            Take = PageSize;
        }
    }
}