using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
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

        public const string ActiveTraceName = "HttpReports-Trace-Context";

        public const string ActiveTraceId = "HttpReports-Trace-Id";

        public const string ActiveSpanId = "HttpReports-Trace-SpanId";

        public const string HttpReportsSpanId = "HttpReports-Trace-Id";

        public const string ActiveParentSpanId = "HttpReports-Trace-ParentSpan-Id"; 
        
        public const string Policy = "HttpReports.Dashboard.Policy";  

        public const string HttpReportsDefaultHealth = "/HttpReportsDefaultHealth";

        public const int DeferTaskMinutes = 3;

        public const string ActiveParentSpanService = "HttpReports-Trace-ParentSpan-Service";


        public const string ActiveSpanService = "HttpReports-Trace-Span-Service";


        public const string AuthToken = "HttpReports-AuthToken";


        public const string ALLTag = "ALL";

        public const string ActiveTraceCreateTime = "HttpReports.Trace.CreateDateTime";


        public const string HttpReportsTraceCost = "HttpReports.Trace.Cost"; 


        public const string HttpReportsHttpClient = "HttpReports.HttpClient";

        public const string ClearDataCornLike = "0 0 0 * * ? ";

        public const string HealthCheckCornLike = "0 0/1 * * * ? ";

        public const string HttpReportsGrpcRequest = "HttpReports.GrpcRequest";

        public const string HttpReportsGrpcResponse = "HttpReports.GrpcResponse";

        public const string Language = "Language";

        public const string DefaultLanguage = "zh-cn";

        public const string StaticFilesRoot = "HttpReports.Dashboard";

        public const string StaticUIRoot = "/HttpReportsStaticFiles/UI";

        public const string HttpReportsServerRegister = "/HttpReportsServerRegister";

        public const int PerformanceInerval = 10000;  
      
        public const string TransportType = "TransportType";

        public const string HttpCollectorEndpoint = "/HttpReportsHttpCollector"; 

    }
}
