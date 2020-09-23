using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HttpReports.Core; 
using Microsoft.AspNetCore.Http;  
using Newtonsoft.Json;

namespace HttpReports
{
    internal class DefaultRequestBuilder : IRequestBuilder
    {
        protected HttpReportsOptions Options;

        public DefaultRequestBuilder(HttpReportsOptions options)
        {
            Options = options;
        }


        public (RequestInfo, RequestDetail) Build(HttpContext context, Stopwatch stopwatch)
        {
            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();

            if (IsFilterRequest(context)) return (null, null);

            // Build RequestInfo  
            Uri uri = new Uri(Options.Server);
            var request = new RequestInfo();

            var remoteIP = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();

            request.RemoteIP = remoteIP.IsEmpty() ? context.Connection.RemoteIpAddress?.MapToIPv4()?.ToString() : remoteIP;
            request.Instance = uri.Host + ":" + uri.Port;
            request.LoginUser = context.User?.Identity?.Name;
            request.StatusCode = context.Response.StatusCode;
            request.Method = context.Request.Method;
            request.Url = context.Request.Path;
            request.RequestType = (context.Request.ContentType ?? string.Empty).Contains("grpc") ? "grpc" : "http";
            request.Milliseconds = ToInt32(stopwatch.ElapsedMilliseconds);
            request.CreateTime = context.Items[BasicConfig.ActiveTraceCreateTime].ToDateTime();

            path = path.Replace(@"///", @"/").Replace(@"//", @"/");

            var (requestInfo, requestDetail) = Build(context, request, path);

            return (ParseRequestInfo(requestInfo), ParseRequestDetail(requestDetail));
        }

        protected (RequestInfo, RequestDetail) Build(HttpContext context, RequestInfo request, string path)
        {
            request.Service = Options.Service.Substring(0, 1).ToUpper() + Options.Service.Substring(1);
            request.Route = GetRoute(path);

            RequestDetail requestDetail = GetRequestDetail(context, request);

            requestDetail.RequestId = request.Id = context.GetTraceId();

            request.ParentId = context.GetTraceParentId();
            request.ParentService = context.GetTraceParentService();

            return (request, requestDetail);
        }

        protected RequestDetail GetRequestDetail(HttpContext context, RequestInfo request)
        {
            RequestDetail model = new RequestDetail();

            if (context.Request != null)
            {
                model.Id = context.GetUniqueId();
                model.RequestId = request.Id;

                Dictionary<string, string> cookies = context.Request.Cookies.ToList().ToDictionary(x => x.Key, x => x.Value);

                if (cookies != null && cookies.Count > 0)
                {
                    model.Cookie = JsonConvert.SerializeObject(cookies);
                }

                Dictionary<string, string> headers = context.Request.Headers.ToList().ToDictionary(x => x.Key, x => x.Value.ToString());

                if (headers != null && headers.Count > 0)
                {
                    model.Header = HttpUtility.HtmlDecode(JsonConvert.SerializeObject(headers));
                }

                if (context.Items.ContainsKey(BasicConfig.HttpReportsGlobalException))
                {
                    Exception ex = context.Items[BasicConfig.HttpReportsGlobalException] as Exception;

                    if (ex != null)
                    {
                        model.ErrorMessage = ex.Message;
                        model.ErrorStack = HttpUtility.HtmlDecode(ex.StackTrace);
                    }

                    context.Items.Remove(BasicConfig.HttpReportsGlobalException);
                }

                if (context.Items.ContainsKey(BasicConfig.HttpReportsRequestBody))
                {
                    string requestBody = context.Items[BasicConfig.HttpReportsRequestBody] as string;

                    if (requestBody != null)
                    {
                        model.RequestBody = requestBody;
                    }

                    context.Items.Remove(BasicConfig.HttpReportsRequestBody);
                }

                if (context.Items.ContainsKey(BasicConfig.HttpReportsResponseBody))
                {
                    string responseBody = context.Items[BasicConfig.HttpReportsResponseBody] as string;

                    if (responseBody != null)
                    {
                        model.ResponseBody = responseBody;
                    }

                    context.Items.Remove(BasicConfig.HttpReportsResponseBody);
                }

                model.CreateTime = context.Items[BasicConfig.ActiveTraceCreateTime].ToDateTime();

                model.Scheme = context.Request.Scheme;
                model.QueryString = HttpUtility.UrlDecode(context.Request.QueryString.Value);
            }

            return model;
        }

        protected IEnumerable<IRequestChain> GetRequestChains(HttpContext context, RequestInfo request)
        {
            var list = context.Items.ToList();

            return null;
        }

        private RequestInfo ParseRequestInfo(RequestInfo request)
        {
            if (request.Service == null) request.Service = string.Empty;
            if (request.Route == null) request.Route = string.Empty;
            if (request.Url == null) request.Url = string.Empty;
            if (request.Method == null) request.Method = string.Empty;
            if (request.RemoteIP == null) request.RemoteIP = string.Empty;
            if (request.LoginUser == null) request.LoginUser = string.Empty;

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

        protected string GetRoute(string path)
        {
            string route = path;

            var list = path.Split('/');

            if (IsNumber(list.ToList().Last()))
            {
                route = route.Substring(0, route.Length - list.ToList().Last().Length - 1);
            }

            if (route.Contains("?"))
            {
                route = route.Split('?').FirstOrDefault();
            }

            return route;
        }

        protected string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        protected string CutString(string str, int count)
        {
            if (str.IsEmpty())
            {
                return string.Empty;
            }

            if (str.Length > count)
            {
                return str.Substring(count);
            }
            else
            {
                return str;
            }
        }
    }
}
