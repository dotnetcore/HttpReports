using System;
using System.Diagnostics;

namespace HttpReports.Storage.FilterOptions
{
    /// <summary>
    /// 请求信息搜索过滤选项
    /// </summary>
    [DebuggerStepThrough]
    public class RequestInfoSearchFilterOption : INodeFilterOption, IStatusCodeFilterOption, ITakeFilterOption, ITimeSpanFilterOption, IOrderFilterOption<RequestInfoFields>
    {
        private int _page;
        private int _pageSize;
        public string[] Nodes { get; set; }
        public int[] StatusCodes { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public RequestInfoFields Field { get; set; } = RequestInfoFields.Id;
        public bool IsAscend { get; set; }
        public string StartTimeFormat { get; set; } = "yyyy-MM-dd";
        public string EndTimeFormat { get; set; } = "yyyy-MM-dd";
        public bool IsOrderByField { get; set; } = true;

        public string TraceId { get; set; }

        public string IP { get; set; }

       
        public string Url { get; set; }

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

        public string Service { get; set; }
        public string LocalIP { get; set; }
        public int LocalPort { get; set; }

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

        public string GetOrderField() => Field.ToString("g");
    }
}