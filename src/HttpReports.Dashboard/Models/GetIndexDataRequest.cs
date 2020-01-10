namespace HttpReports.Dashboard.Models
{
    public class GetIndexDataRequest
    {
        public string Node { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public string Day { get; set; }

        public string Month { get; set; }

        public string Year { get; set; }

        public int TOP { get; set; }

        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDesc { get; set; }
    }

    public class GetIndexDataResponse
    {
        /// <summary>
        /// 平均处理时间
        /// </summary>
        public string ART { get; set; }

        /// <summary>
        /// 总请求次数
        /// </summary>
        public string Total { get; set; }

        /// <summary>
        /// 404
        /// </summary>
        public string Code404 { get; set; }

        /// <summary>
        /// 500
        /// </summary>

        public string Code500 { get; set; }

        /// <summary>
        /// 错误占比
        /// </summary>
        public string ErrorPercent { get; set; }

        /// <summary>
        /// 接口数量 - 已调用
        /// </summary>
        public int APICount { get; set; }
    }
}