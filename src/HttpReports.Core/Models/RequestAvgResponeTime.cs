namespace HttpReports.Models
{
    /// <summary>
    /// 请求的平均请求响应时间
    /// </summary>
    public class RequestAvgResponeTime
    {
        public string Url { get; set; }

        public float Time { get; set; }
    }
}