namespace HttpReports.Models
{
    /// <summary>
    /// Url的请求总数信息
    /// </summary>
    public class UrlRequestCount
    {
        public string Url { get; set; }

        public int Total { get; set; }
    }
}