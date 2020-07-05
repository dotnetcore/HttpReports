using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttpReports
{
    internal class DefaultHttpReportsMiddleware
    {
        private readonly RequestDelegate _next;

        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IHttpInvokeProcesser InvokeProcesser { get; }

        private HttpReportsOptions Options { get; }

        public ILogger<DefaultHttpReportsMiddleware> Logger { get; }

        public DefaultHttpReportsMiddleware(RequestDelegate next, IOptions<HttpReportsOptions> options, IRequestInfoBuilder requestInfoBuilder, IHttpInvokeProcesser invokeProcesser, ILogger<DefaultHttpReportsMiddleware> logger)
        {
            _next = next;
            Logger = logger;
            RequestInfoBuilder = requestInfoBuilder;
            InvokeProcesser = invokeProcesser;
            Options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value == "/")
            {
                await _next(context);
                return;
            }

            if (!context.Request.ContentType.IsEmpty() && context.Request.ContentType.Contains("application/grpc"))
            {
                await InvokeGrpcAsync(context);
            }
            else
            {
                await InvokeHttpAsync(context);
            }
        }


        private async Task InvokeHttpAsync(HttpContext context)
        {
            if (FilterRequest(context))
            {
                await _next(context);
                return;
            } 

            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            ConfigTrace(context);

            string requestBody = await GetRequestBodyAsync(context);

            var originalBodyStream = context.Response.Body;

            var responseMemoryStream = new MemoryStream();

            try
            {
                context.Response.Body = responseMemoryStream;

                await _next(context);

            } 
            finally
            {
                stopwatch.Stop();

                string responseBody = await GetResponseBodyAsync(context);

                context.Items.Add(BasicConfig.HttpReportsRequestBody, requestBody);
                context.Items.Add(BasicConfig.HttpReportsResponseBody, responseBody);

                await responseMemoryStream.CopyToAsync(originalBodyStream);

                responseMemoryStream.Dispose();

                if (!string.IsNullOrEmpty(context.Request.Path))
                {
                    InvokeProcesser.Process(context, stopwatch);
                }

            }

        }

        private async Task InvokeGrpcAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            ConfigTrace(context);

            try
            {
                await _next(context);

            }
            finally
            {
                stopwatch.Stop();

                string requestBody = string.Empty;
                string responseBody = string.Empty;

                if (context.Items.ContainsKey(BasicConfig.HttpReportsGrpcRequest))
                {
                    requestBody = JsonConvert.SerializeObject(context.Items[BasicConfig.HttpReportsGrpcRequest]);
                }

                if (context.Items.ContainsKey(BasicConfig.HttpReportsGrpcResponse))
                {
                    responseBody = JsonConvert.SerializeObject(context.Items[BasicConfig.HttpReportsGrpcResponse]);
                }

                context.Items.Add(BasicConfig.HttpReportsRequestBody, requestBody);
                context.Items.Add(BasicConfig.HttpReportsResponseBody, responseBody);

                if (!string.IsNullOrEmpty(context.Request.Path))
                {
                    InvokeProcesser.Process(context, stopwatch);
                }
            }
        }


        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            try
            {
                string result = string.Empty;

                context.Request.EnableBuffering();

                var requestReader = new StreamReader(context.Request.Body,System.Text.Encoding.UTF8);

                result = await requestReader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                return HttpUtility.HtmlDecode(result);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
                return string.Empty;
            }
        }

        private async Task<string> GetResponseBodyAsync(HttpContext context)
        {
            try
            {
                if (FilterStaticFiles(context))
                {
                    context.Response.Body.Seek(0, SeekOrigin.Begin);
                    return string.Empty;
                } 
                

                string result = string.Empty;

                context.Response.Body.Seek(0, SeekOrigin.Begin);

                var responseReader = new StreamReader(context.Response.Body, System.Text.Encoding.UTF8);

                result = await responseReader.ReadToEndAsync();

                context.Response.Body.Seek(0, SeekOrigin.Begin);

                return HttpUtility.UrlDecode(result);

            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
                return string.Empty;
            }
        }

        private void ConfigTrace(HttpContext context)
        { 
            var parentId = context.Request.Headers.ContainsKey(BasicConfig.ActiveTraceId) ?
                context.Request.Headers[BasicConfig.ActiveTraceId].ToString() : string.Empty;

            Activity.Current = null;
            Activity activity = new Activity(BasicConfig.ActiveTraceName);  

            activity.Start();  
            activity.SetParentId(parentId);   

            activity.AddBaggage(BasicConfig.ActiveTraceId, activity.Id);  
            activity.AddBaggage(BasicConfig.ActiveSpanId,activity.SpanId.ToHexString()); 
            if (!parentId.IsEmpty())
            {
                activity.AddBaggage(BasicConfig.ActiveParentSpanId, activity.ParentSpanId.ToHexString());
            }
                

            context.Items.Add(BasicConfig.ActiveTraceCreateTime, DateTime.Now);
            context.Items.Add(BasicConfig.ActiveTraceId, activity.Id);
            context.Items.Add(BasicConfig.ActiveSpanId, activity.SpanId.ToHexString()); 
            if (!parentId.IsEmpty())
            {
                context.Items.Add(BasicConfig.ActiveParentSpanId, activity.ParentSpanId.ToHexString());
            }  

            context.Response.Headers.Add(BasicConfig.ActiveSpanId, activity.SpanId.ToHexString());

        }

        private bool FilterStaticFiles(HttpContext context)
        { 
            if (!context.Request.ContentType.IsEmpty() && context.Request.ContentType.Contains("application/grpc"))
                return false;

            if (context.Request.Method.ToLowerInvariant() == "options")
                return true;

            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("."))
            {
                return true;
            }

            return false;
        }

        private bool FilterRequest(HttpContext context)
        {
            if (Options.FilterRequest == null || Options.FilterRequest.Count() == 0)
            {
                return false;
            } 

            var path = context.Request.Path.Value.ToLowerInvariant();

            if (path.StartsWith(BasicConfig.TransportPath.ToLowerInvariant()))
            {
                return true;
            }


            return MatchRequestRule();

            bool MatchRequestRule()
            {
                bool result = false;

                foreach (var item in Options.FilterRequest)
                {
                    var rule = item.ToLowerInvariant();

                    var ruleList = rule.ToList();

                    if (!ruleList.Where(x => x == '%').Any())
                    {
                        if (path == rule)
                        {
                            return true;
                        } 

                    }
                    else if (ruleList.Where(x => x == '%').Count() >= 2)
                    {
                        if (path.Contains(rule.Replace("%", "")))
                        {
                            return true;
                        }
                    }
                    else if (ruleList.Where(x => x == '%').Count() == 1 && rule.LastOrDefault() == '%')
                    {
                        if (path.StartsWith(rule.Replace("%", "")))
                        {
                            return true;
                        }
                    } 
                    else if (ruleList.Where(x => x == '%').Count() == 1 && rule.FirstOrDefault() == '%')
                    {
                        if (path.EndsWith(rule.Replace("%", "")))
                        {
                            return true;
                        }
                    }
                }

                return result;
            }
        } 
      
    }
}