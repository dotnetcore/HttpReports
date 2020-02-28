using System;

namespace HttpReports
{
    /// <summary>
    /// 请求信息基础类
    /// </summary>
    public interface IRequestInfo
    { 

        DateTime CreateTime { get; set; }

        /// <summary>
        /// ES 设置GUID 为ID
        /// </summary>
        string Id { get; set; }
        string IP { get; set; }
        string Method { get; set; }
        int Milliseconds { get; set; }
        string Node { get; set; }
        string Route { get; set; }
        int StatusCode { get; set; }
        string Url { get; set; }
    }
}