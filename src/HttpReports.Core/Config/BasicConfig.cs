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

        public const int HttpReportsFieldMaxLength = 80000;

        public const int ExpireDay = 7;

        public const string ActiveTraceName = "HttpReports.Trace.Context";

        public const string ActiveTraceId = "HttpReports.Trace.Id";

        public const string ActiveSpanId = "HttpReports.Trace.Span.Id";

        public const string ActiveParentSpanId = "HttpReports.Trace.ParentSpan.Id";


        public const string AuthToken = "HttpReports.Auth.Token";


        public const string ALLTag = "ALL";

        public const string ActiveTraceCreateTime = "HttpReports.Trace.CreateTime";

        public const string HttpReportsHttpClient = "HttpReports.HttpClient";

        public const string ClearDataCornLike = "0 0 0 * * ? ";

        public const string HttpReportsGrpcRequest = "HttpReports.GrpcRequest";

        public const string HttpReportsGrpcResponse = "HttpReports.GrpcResponse";

        public const string Language = "Language";

        public const string StaticFilesRoot = "HttpReports.Dashboard";

        public const string HttpReportsServerRegister = "/HttpReportsServerRegister";

        public const int PerformanceInerval = 10000; 

        public const string TransportRequestBag = "/HttpReportsTransport/RequestBag";

        public const string TransportType = "TransportType";

        public const string TransportPath = "/DataTransportPath";

        public const string TransportPerformance = "/HttpReportsTransport/Performance";

    }
}
