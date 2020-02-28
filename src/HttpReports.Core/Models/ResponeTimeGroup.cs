namespace HttpReports.Models
{
    /// <summary>
    /// 响应时间分组
    /// </summary>
    public class ResponeTimeGroup
    {
        public string Name { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public int Total { get; set; }
    }
}