using System;
using System.Diagnostics;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

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
        
        protected abstract IRequestInfo Build(IRequestInfo request, string path);

        public IRequestInfo Build(HttpContext context, Stopwatch stopwatch)
        { 
            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant(); 

            if (Options.FilterStaticFiles && path.Contains("."))
            { 
                return null;
            }  

            // 创建请求信息
            var request = ModelCreator.CreateRequestInfo();
            request.IP = context.Connection.RemoteIpAddress.ToString();
            request.StatusCode = context.Response.StatusCode;
            request.Method = context.Request.Method;
            request.Url = context.Request.Path;
            request.Milliseconds = ToInt32(stopwatch.ElapsedMilliseconds);
            request.CreateTime = DateTime.Now;

            path = path.Replace(@"///",@"/").Replace(@"//", @"/"); 

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
            string Node = Options.Node; 

            var arr = path.Substring(1).Split('/');   

            if (arr.Length > 1 && (arr[1] ?? string.Empty).ToLower() == Options.ApiPoint.ToLower())
            {
                Node = arr[0];
            }

            Node = Node.Substring(0, 1).ToUpper() + Node.Substring(1).ToLower();

            return Node;
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