using System;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {
        /// <summary>
        /// 默认为 api
        /// </summary>
        public string ApiPoint { get; set; } = "api";

        /// <summary>
        /// 服务节点名称
        /// </summary>
        public string Node { get; set; } = "default";

        public HttpReportsOptions Value => this;
    } 
}