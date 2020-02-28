namespace HttpReports.Models
{
    /// <summary>
    /// http状态总数信息
    /// </summary>
    public class StatusCodeCount
    {
        public int Code { get; set; }

        public int Total { get; set; }
    }
}