using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HttpReports.RequestInfoBuilder
{
    internal class DefaultRequestInfoBuilder : BaseRequestInfoBuilder
    {
        public DefaultRequestInfoBuilder(IModelCreator modelCreator, IOptions<HttpReportsOptions> options) : base(modelCreator, options)
        {

        }


        protected override (IRequestInfo, IRequestDetail) Build(HttpContext context, IRequestInfo request, string path)
        {
            if (Options.Node.IsEmpty())
            {
                Options.Node = "Default";
            }

            request.Node = Options.Node.Substring(0, 1).ToUpper() + Options.Node.Substring(1).ToLower();
            request.Route = GetRoute(path); 

            IRequestDetail requestDetail = GetRequestDetail(context, request);

            requestDetail.RequestId = request.Id = System.Diagnostics.Activity.Current.SpanId.ToString();
            request.ParentId = Activity.Current.GetBaggageItem(BasicConfig.ActiveTraceName) ?? string.Empty;

            return (request, requestDetail);
        }

        private IRequestDetail GetRequestDetail(HttpContext context, IRequestInfo request)
        {
            IRequestDetail model = new RequestDetail();

            if (context.Request != null)
            {
                model.Id = MD5_16(Guid.NewGuid().ToString());
                model.RequestId = request.Id;

                Dictionary<string, string> cookies = context.Request.Cookies.ToList().ToDictionary(x => x.Key, x => x.Value);

                if (cookies != null && cookies.Count > 0)
                {
                    model.Cookie = JsonConvert.SerializeObject(cookies);
                }

                Dictionary<string, string> headers = context.Request.Headers.ToList().ToDictionary(x => x.Key, x => x.Value.ToString());

                if (headers != null && headers.Count > 0)
                {
                    model.Header = JsonConvert.SerializeObject(headers);
                }  

                if (context.Items.ContainsKey(BasicConfig.HttpReportsGlobalException))
                {
                    Exception ex = context.Items[BasicConfig.HttpReportsGlobalException] as Exception;

                    if (ex != null)
                    {
                        model.ErrorMessage = ex.Message;
                        model.ErrorStack = ex.StackTrace; 
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

                model.CreateTime = DateTime.Now;

                model.Scheme = context.Request.Scheme;
                model.QueryString = context.Request.QueryString.Value;
            }

            return model;
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

        private string CutString(string str,int count)
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
