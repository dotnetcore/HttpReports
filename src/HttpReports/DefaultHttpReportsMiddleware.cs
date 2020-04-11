using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace HttpReports
{
    internal class DefaultHttpReportsMiddleware
    {
        private readonly RequestDelegate _next;

        public IRequestInfoBuilder RequestInfoBuilder { get; }

        public IHttpInvokeProcesser InvokeProcesser { get; }

        public DefaultHttpReportsMiddleware(RequestDelegate next, IRequestInfoBuilder requestInfoBuilder, IHttpInvokeProcesser invokeProcesser)
        {
            _next = next;
            RequestInfoBuilder = requestInfoBuilder;
            InvokeProcesser = invokeProcesser;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method.ToUpper() == "OPTIONS")
            {
                await _next(context);
                return; 
            } 

            var stopwatch = Stopwatch.StartNew();
            stopwatch.Start();

            ConfigTrace();

            string requestBody = await GetRequestBodyAsync(context);

            var originalBodyStream = context.Response.Body;

            using (var responseMemoryStream = new MemoryStream())
            {
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

                    originalBodyStream.Dispose();

                    if (!string.IsNullOrEmpty(context.Request.Path))
                    {
                        InvokeProcesser.Process(context, stopwatch);
                    }

                }  
            }

        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            string result = string.Empty;

            context.Request.EnableBuffering();

            var requestReader = new StreamReader(context.Request.Body);

            result = await requestReader.ReadToEndAsync();

            context.Request.Body.Position = 0;

            return result;

        }

        private async Task<string> GetResponseBodyAsync(HttpContext context)
        {
            string result = string.Empty;

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            var responseReader = new StreamReader(context.Response.Body);

            result = await responseReader.ReadToEndAsync();

            context.Response.Body.Seek(0, SeekOrigin.Begin);

            return result;

        }

        private void ConfigTrace()
        {
            var parentId = Activity.Current.GetBaggageItem(BasicConfig.ActiveTraceName);

            if (string.IsNullOrEmpty(parentId))
            {
                Activity activity = new Activity(BasicConfig.ActiveTraceName); 
                activity.Start(); 
                activity.AddBaggage(BasicConfig.ActiveTraceId, activity.Id); 
            }
            else
            {
                Activity activity = new Activity(BasicConfig.ActiveTraceName);
                activity.SetParentId(parentId); 
                activity.Start();
                activity.AddBaggage(BasicConfig.ActiveTraceId, activity.Id);  
            }
        }

    }
}