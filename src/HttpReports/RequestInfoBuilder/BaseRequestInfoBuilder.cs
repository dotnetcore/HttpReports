using HttpReports.Core;
using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;

namespace HttpReports
{
    internal abstract class BaseRequestInfoBuilder : IRequestInfoBuilder
    {
        protected HttpReportsOptions Options { get; }
        protected IModelCreator ModelCreator { get; }

        public BaseRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options)
        {
            Options = options.Value;
            ModelCreator = modelCreator;
        }

        protected abstract (RequestInfo, RequestDetail) Build(HttpContext context, RequestInfo request, string path);

        public (RequestInfo, RequestDetail) Build(HttpContext context, Stopwatch stopwatch)
        {    
            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();

            if (IsFilterRequest(context)) return (null, null); 

            // Build RequestInfo

            Uri uri = new Uri(Options.Urls);
            var request = ModelCreator.CreateRequestInfo();

            var remoteIP = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            request.RemoteIP = remoteIP.IsEmpty() ? context.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() : remoteIP;
            request.Instance = uri.Host + ":" + uri.Port; 
            request.StatusCode = context.Response.StatusCode; 
            request.Method = context.Request.Method;
            request.Url = context.Request.Path;
            request.RequestType = (context.Request.ContentType ?? string.Empty).Contains("grpc") ? "grpc" : "http";
            request.Milliseconds = ToInt32(stopwatch.ElapsedMilliseconds);
            request.CreateTime = context.Items[BasicConfig.ActiveTraceCreateTime].ToDateTime(); 
           
            path = path.Replace(@"///", @"/").Replace(@"//", @"/");

            var (requestInfo, requestDetail ) = Build(context, request, path);

            return (ParseRequestInfo(requestInfo), ParseRequestDetail(requestDetail));
        }

        private RequestInfo ParseRequestInfo(RequestInfo request)
        {
            if (request.Service == null) request.Service = string.Empty;
            if (request.Route == null) request.Route = string.Empty;
            if (request.Url == null) request.Url = string.Empty;
            if (request.Method == null) request.Method = string.Empty;
            if (request.RemoteIP == null) request.RemoteIP = string.Empty;

            return request;
        }

        private RequestDetail ParseRequestDetail(RequestDetail request)
        {
            if (request.Scheme == null) request.Scheme = string.Empty;
            if (request.QueryString == null) request.QueryString = string.Empty;
            if (request.Header == null) request.Header = string.Empty;
            if (request.Cookie == null) request.Cookie = string.Empty;
            if (request.RequestBody == null) request.RequestBody = string.Empty;
            if (request.ResponseBody == null) request.ResponseBody = string.Empty;
            if (request.ErrorMessage == null) request.ErrorMessage = string.Empty;
            if (request.ErrorStack == null) request.ErrorStack = string.Empty;

            int max = BasicConfig.HttpReportsFieldMaxLength;

            if (request.QueryString.Length > max)
            {
                request.QueryString = request.QueryString.Substring(0, max);
            }

            if (request.Header.Length > max)
            {
                request.Header = request.Header.Substring(0, max);
            }

            if (request.Cookie.Length > max)
            {
                request.Cookie = request.Cookie.Substring(0, max);
            }

            if (request.RequestBody.Length > max)
            {
                request.RequestBody = request.RequestBody.Substring(0, max);
            }

            if (request.ResponseBody.Length > max)
            {
                request.ResponseBody = request.ResponseBody.Substring(0, max);
            }

            if (request.ErrorMessage.Length > max)
            {
                request.ErrorMessage = request.ErrorMessage.Substring(0, max);
            }

            if (request.ErrorStack.Length > max)
            {
                request.ErrorStack = request.ErrorStack.Substring(0, max);
            }

            return request;
        } 


        private bool IsFilterRequest(HttpContext context)
        {
            bool result = false;

            if (!context.Request.ContentType.IsEmpty() && context.Request.ContentType.Contains("application/grpc"))
                return false;

            if (context.Request.Method.ToLowerInvariant() == "options")
                return true;

            if (!Options.FilterStaticFile)
                return false;

            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();

            if (path.Contains("."))
            {
                var fileType = path.Split('.').Last();

                if (fileType != "html" && fileType != "aspx")
                    return true;
            }

            return result;
        }

        protected static int ToInt32(long value)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                return -1;
            }
            return (int)value == 0 ? 1 : (int)value;
        }

        protected static bool IsNumber(string str)
        {
            return int.TryParse(str, out _);
        }
    }
}