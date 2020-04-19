using System;
using System.Diagnostics;
using System.IO;
using HttpReports.Core.Config;
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
        
        protected abstract (IRequestInfo,IRequestDetail) Build(HttpContext context,IRequestInfo request, string path); 

        public (IRequestInfo,IRequestDetail) Build(HttpContext context, Stopwatch stopwatch)
        {  
            var path = (context.Request.Path.Value ?? string.Empty).ToLowerInvariant(); 

            if (Options.FilterStaticFiles && path.Contains("."))
            { 
                return (null,null);
            }  

            // Build RequestInfo 
            var request = ModelCreator.CreateRequestInfo();
            request.IP = context.Connection.RemoteIpAddress.ToString();
            request.Port = context.Connection.RemotePort;
            request.LocalIP = context.Connection.LocalIpAddress.ToString();
            request.LocalPort = context.Connection.LocalPort;
            request.StatusCode = context.Response.StatusCode;
            request.Method = context.Request.Method;
            request.Url = context.Request.Path;
            request.RequestType = context.Request.ContentType == "application/grpc" ? "grpc" : "http";
            request.Milliseconds = ToInt32(stopwatch.ElapsedMilliseconds);
            request.CreateTime = context.Items[BasicConfig.ActiveTraceCreateTime].ToDateTime();

            path = path.Replace(@"///",@"/").Replace(@"//", @"/");  

            var (requestInfo,requestDetail) = Build(context, request, path);

            return (ParseRequestInfo(requestInfo),ParseRequestDetail(requestDetail));

        }

        private IRequestInfo ParseRequestInfo(IRequestInfo request)
        {
            if (request.Node == null) request.Node = string.Empty;
            if (request.Route == null) request.Route = string.Empty;
            if (request.Url == null) request.Url = string.Empty;
            if (request.Method == null) request.Method = string.Empty;
            if (request.IP == null) request.IP = string.Empty; 

            return request;
        }

        private IRequestDetail ParseRequestDetail(IRequestDetail request)
        {
            if (request.Scheme == null) request.Scheme = string.Empty;
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
                request.QueryString = request.QueryString.Substring(0,max);
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


        protected static int ToInt32(long value)
        {
            if (value < int.MinValue || value > int.MaxValue)
            {
                return -1;
            }
            return (int)value == 0 ? 1:(int)value;
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