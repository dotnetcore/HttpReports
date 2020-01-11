using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttpReports
{
    internal abstract class BaseRequestInfoBuilder : IRequestInfoBuilder
    {
        protected HttpReportsOptions Options { get; }

        public BaseRequestInfoBuilder(IOptions<HttpReportsOptions> options)
        {
            Options = options.Value;
        }

        //TODO 此函数是否只需要传path？
        protected abstract RequestInfo Build(RequestInfo request, string path);

        public IRequestInfo Build(HttpContext context, Stopwatch stopwatch)
        {
            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant();

            // 创建请求信息
            RequestInfo request = new RequestInfo
            {
                IP = context.Connection.RemoteIpAddress.ToString(),
                StatusCode = context.Response.StatusCode,
                Method = context.Request.Method,
                Url = context.Request.Path,
                Milliseconds = ToInt32(stopwatch.ElapsedMilliseconds),
                CreateTime = DateTime.Now
            };

            return Build(request, path);
        }

        protected static int ToInt32(long value)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                return -1;
            }
            return (int)value;
        }

        /// <summary>
        /// 通过请求地址 获取服务节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Default"></param>
        /// <returns></returns>
        protected string GetNode(string path)
        {
            string Default = Options.Node;

            var arr = path.Substring(1).Split('/');

            if (arr.Length > 0 && arr[1] == Options.ApiPoint)
            {
                Default = arr[0];
            }

            Default = Default.Substring(0, 1).ToUpper() + Default.Substring(1).ToLower();

            return Default;
        }

        protected static bool IsNumber(string str)
        {
            try
            {
                int i = Convert.ToInt32(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}