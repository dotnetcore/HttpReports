using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HttpReports.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttpReports
{
    internal class DefaultHttpReportsMiddleware
    {
        private readonly RequestDelegate Next;

        public IRequestBuilder RequestBuilder { get; }

        public IRequestProcesser InvokeProcesser { get; }

        private HttpReportsOptions Options { get; }

        public ILogger<DefaultHttpReportsMiddleware> Logger { get; }

        public DefaultHttpReportsMiddleware(RequestDelegate next, IOptions<HttpReportsOptions> options, IRequestBuilder requestBuilder, IRequestProcesser invokeProcesser, ILogger<DefaultHttpReportsMiddleware> logger)
        {
            Next = next;
            Logger = logger;
            RequestBuilder = requestBuilder;
            InvokeProcesser = invokeProcesser;
            Options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if ((context.Request.Path.HasValue && context.Request.Path.Value == "/"))
            {
                await Next(context);
                return;
            }

            if (!context.Request.ContentType.IsEmpty() && context.Request.ContentType.Contains("application/grpc"))
            {
                await InvokeGrpcAsync(context);
            } 
            else if(!context.Request.Path.Value.Contains("."))
            {
                await InvokeHttpAsync(context);
            }
            else
            {
                await Next(context);
            } 
        }


        private async Task InvokeHttpAsync(HttpContext context)
        {
            if (FilterRequest(context))
            {
                await Next(context);
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

                await Next(context); 
            } 
            finally
            { 
                stopwatch.Stop();

                string responseBody = await GetResponseBodyAsync(context);

                context.Items.Add(BasicConfig.HttpReportsTraceCost, stopwatch.ElapsedMilliseconds); 
                context.Items.Add(BasicConfig.HttpReportsRequestBody, requestBody);
                context.Items.Add(BasicConfig.HttpReportsResponseBody, responseBody); 

                if (responseMemoryStream.CanRead && responseMemoryStream.CanSeek)
                {
                    await responseMemoryStream.CopyToAsync(originalBodyStream);

                    responseMemoryStream.Dispose();

                }  

                if (!string.IsNullOrEmpty(context.Request.Path))
                {
                    InvokeProcesser.Process(context);
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
                await Next(context);

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

                context.Items.Add(BasicConfig.HttpReportsTraceCost, stopwatch.ElapsedMilliseconds);
                context.Items.Add(BasicConfig.HttpReportsRequestBody, requestBody);
                context.Items.Add(BasicConfig.HttpReportsResponseBody, responseBody);

                if (!string.IsNullOrEmpty(context.Request.Path))
                {
                    InvokeProcesser.Process(context);
                }
            }
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            try
            {  
                if (context.Request.ContentType.IsEmpty() || !context.Request.ContentType.Contains("application/json") || !Options.WithRequest 
                    || 
                    (context.Request.ContentLength.HasValue && context.Request.ContentLength.Value > Options.MaxBytes ))
                { 
                    return string.Empty;
                }
 
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
                if (context.Response.ContentType.IsEmpty() || !context.Response.ContentType.Contains("application/json") || !Options.WithResponse || context.Response.Body.Length > Options.MaxBytes)
                {
                    if (context.Response.Body.CanSeek)
                    {
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                    }  
                    return string.Empty;
                }

                if (FilterStaticFiles(context))
                {
                    if (context.Response.Body.CanSeek)
                    {
                        context.Response.Body.Seek(0, SeekOrigin.Begin);
                    }  
                       
                    return string.Empty;
                }  

                string result = string.Empty;

                context.Response.Body.Seek(0, SeekOrigin.Begin);


                bool compress = false;

                if (context.Response.Headers.ContainsKey("Content-Encoding") && context.Response.Headers["Content-Encoding"].ToString() == "gzip")
                {
                    compress = true;
                }  

                var source = compress ? new GZipStream(context.Request.Body, CompressionMode.Decompress) : context.Response.Body; 

                var responseReader = new StreamReader(source, System.Text.Encoding.UTF8);

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

            // Set Activity
            activity.AddBaggage(BasicConfig.ActiveTraceId, activity.Id);
            activity.AddBaggage(BasicConfig.ActiveSpanId, activity.SpanId.ToHexString());
            if (!parentId.IsEmpty())
            {
                activity.AddBaggage(BasicConfig.ActiveParentSpanId, activity.ParentSpanId.ToHexString());
            }

            // Set Context
            context.Items.Add(BasicConfig.ActiveTraceCreateTime, DateTime.Now);
            context.Items.Add(BasicConfig.ActiveTraceId, activity.Id);
            context.Items.Add(BasicConfig.ActiveSpanId, activity.SpanId.ToHexString());
            if (!parentId.IsEmpty())
            {
                context.Items.Add(BasicConfig.ActiveParentSpanId, activity.ParentSpanId.ToHexString());
            }



            var parentService = context.Request.Headers.ContainsKey(BasicConfig.ActiveParentSpanService) ?
               context.Request.Headers[BasicConfig.ActiveParentSpanService].ToString() : string.Empty;   

            activity.AddBaggage(BasicConfig.ActiveSpanService,Options.Service);
            context.Items.Add(BasicConfig.ActiveSpanService, Options.Service);

            if (!parentService.IsEmpty())
            {
                activity.AddBaggage(BasicConfig.ActiveParentSpanService, parentService);
                context.Items.Add(BasicConfig.ActiveParentSpanService, parentService);
            }  

            // Set Response
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
            if (context.Request.HasFormContentType && context.Request.Form != null && context.Request.Form.Files != null && context.Request.Form.Files.Any())
            {
                return true;
            }


            var path = context.Request.Path.Value.ToLowerInvariant();

            if (path.StartsWith(BasicConfig.TransportPath.ToLowerInvariant()) || path.StartsWith(BasicConfig.HttpReportsDefaultHealth.ToLowerInvariant()))
            {
                return true;
            }


            if (Options.RequestFilter == null || Options.RequestFilter.Count() == 0)
            {
                return false;
            }   

            return MatchRequestRule();

            bool MatchRequestRule()
            {
                bool result = false;

                foreach (var item in Options.RequestFilter)
                {
                    var rule = item.ToLowerInvariant();

                    var ruleList = rule.ToList();

                    if (!ruleList.Where(x => x == '*').Any())
                    {
                        if (path == rule)
                        {
                            return true;
                        } 

                    }
                    else if (ruleList.Where(x => x == '*').Count() >= 2)
                    {
                        if (path.Contains(rule.Replace("*", "")))
                        {
                            return true;
                        }
                    }
                    else if (ruleList.Where(x => x == '*').Count() == 1 && rule.LastOrDefault() == '*')
                    {
                        if (path.StartsWith(rule.Replace("*", "")))
                        {
                            return true;
                        }
                    } 
                    else if (ruleList.Where(x => x == '*').Count() == 1 && rule.FirstOrDefault() == '*')
                    {
                        if (path.EndsWith(rule.Replace("*", "")))
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