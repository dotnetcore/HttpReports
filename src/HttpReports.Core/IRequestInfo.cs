using System;

namespace HttpReports
{
    /// <summary>
    /// 请求信息基础类
    /// </summary>
    public interface IRequestInfo
    {   
        string Id { get; set; } 

        string ParentId { get; set; }

        string Service { get; set; }

        string Instance { get; set; }

        string Route { get; set; }

        string Url { get; set; }

        string RequestType { get; set; }

        string Method { get; set; }

        int Milliseconds { get; set; }

        int StatusCode { get; set; }

        string RemoteIP { get; set; } 

        string LoginUser { get; set; }   

        DateTime CreateTime { get; set; }
    }
}