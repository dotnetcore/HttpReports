using System;

using Microsoft.Extensions.Options;

namespace HttpReports
{
    public class HttpReportsOptions : IOptions<HttpReportsOptions>
    {

        public string ApiPoint { get; set; } = "api";


        public string Node { get; set; } = "default";


        public bool UseHome { get; set; } = true;

        public bool FilterStaticFiles { get; set; } = true;

        /// <summary>
        /// 过滤特定IP
        /// </summary>
        public string[] FilterIPAddress { get; set; }

        /// <summary>
        /// 过滤特定路径
        /// </summary>
        public string[] FilterUrls { get; set; }

        public HttpReportsOptions Value => this;
    }
}