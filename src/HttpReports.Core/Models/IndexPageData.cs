namespace HttpReports.Models
{
    /// <summary>
    /// 首页数据
    /// </summary>
    public class IndexPageData
    {
        public double AvgResponseTime { get; set; }

        public int Total { get; set; }

        public int NotFound { get; set; }

        public int ServerError { get; set; }

        public int APICount { get; set; }
        public double ErrorPercent { get; set; }
    }
}