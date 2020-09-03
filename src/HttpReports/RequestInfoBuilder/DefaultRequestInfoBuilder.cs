using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using HttpReports.Core;
using HttpReports.Core.Config;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace HttpReports.RequestInfoBuilder
{
    internal class DefaultRequestInfoBuilder : BaseRequestInfoBuilder
    {  
        public DefaultRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options) : base(modelCreator, options)
        {

        }

        protected override (RequestInfo, RequestDetail) Build(HttpContext context, RequestInfo request, string path)
        {
            if (Options.Service.IsEmpty())
            {
                Options.Service = Options.Node.IsEmpty() ? "Default":Options.Node;
            }

            request.Service = Options.Service.Substring(0, 1).ToUpper() + Options.Service.Substring(1);
            request.Route = GetRoute(path);

            RequestDetail requestDetail = GetRequestDetail(context, request);

            requestDetail.RequestId = request.Id = context.GetTraceId();

            request.ParentId = context.GetTraceParentId();
            request.ParentService = context.GetTraceParentService();

            return (request, requestDetail);
        }

        private RequestDetail GetRequestDetail(HttpContext context,RequestInfo request)
        {
            RequestDetail model = ModelCreator.CreateRequestDetail();

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
                        model.ErrorMessage =  ex.Message;
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


        private IEnumerable<IRequestChain> GetRequestChains(HttpContext context, RequestInfo request)
        {
            var list = context.Items.ToList();    

            return null; 
        }


        private string GetRoute(string path)
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

        private string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        private string CutString(string str, int count)
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