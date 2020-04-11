using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Config
{
    public static class BasicConfig
    { 
        public const string DefaultUserName = "admin"; 
 
        public const string DefaultPassword = "e10adc3949ba59abbe56e057f20f883e";

        public const string LoginCookieId = "HttpReports.Login.User";

        public const string CurrentControllers = "HttpReports,HttpReportsData";

        public const string ElasticSearchIndexName = "httpreports_index_";

        public const string HttpReportsGlobalException = "HttpReportsGlobalException";

        public const string HttpReportsRequestBody = "HttpReportsRequestBody";

        public const string HttpReportsResponseBody = "HttpReportsResponseBody";

        public const int HttpReportsFieldMaxLength = 10000;

        public const int ExpireDay = 30;

        public const string ActiveTraceName = "HttpReports.Trace.Context";

        public const string ActiveTraceId = "HttpReports.Trace.Id";

        public const string ClearDataCornLike = "0 0 0 * * ? "; 
    }
}
