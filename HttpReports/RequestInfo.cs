using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{

    [Table("RequestInfo")]
    public class RequestInfo
    {

        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 服务节点
        /// </summary>
        public string Node { get; set; }


        /// <summary>
        /// 路由
        /// </summary>
        public string Route { get; set; }


        /// <summary>
        /// 请求地址
        /// </summary>
        public string Url { get; set; }


        /// <summary>
        /// GET Or Post
        /// </summary>
        public string Method { get; set; }


        /// <summary>
        /// 请求毫秒数
        /// </summary>
        public int Milliseconds { get; set; }


        /// <summary>
        /// HTTP 状态码
        /// </summary>
        public int StatusCode { get; set; }


        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }
}
