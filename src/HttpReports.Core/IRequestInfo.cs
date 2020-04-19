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
        string IP { get; set; } 

        int Port { get; set; }

        string LocalIP { get; set; }

        int LocalPort { get; set; }

        string Method { get; set; }
        int Milliseconds { get; set; }
        string Node { get; set; }
        string Route { get; set; }
        int StatusCode { get; set; }
        string Url { get; set; }     
        string RequestType { get; set; }
        DateTime CreateTime { get; set; }
    }
}