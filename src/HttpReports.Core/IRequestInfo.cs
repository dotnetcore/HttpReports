using System;

namespace HttpReports
{
    /// <summary>
    /// 请求信息基础类
    /// </summary>
    public interface IRequestInfo
    {
        //TODO 看下是否可优化信息、结构等

        DateTime CreateTime { get; set; }
        int Id { get; set; }
        string IP { get; set; }
        string Method { get; set; }
        int Milliseconds { get; set; }
        string Node { get; set; }
        string Route { get; set; }
        int StatusCode { get; set; }
        string Url { get; set; }
    }
}