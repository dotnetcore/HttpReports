namespace HttpReports.Dashboard.Models
{
    public class GetTopRequest
    {
        public string Node { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public int TOP { get; set; }

        /// <summary>
        /// 是否倒序
        /// </summary>
        public bool IsDesc { get; set; }
    }

    public class GetTopResponse
    {
        public string Url { get; set; }

        public int Total { get; set; }
    }
}