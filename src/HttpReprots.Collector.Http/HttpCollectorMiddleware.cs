using HttpReports;
using HttpReports.Core; 
using HttpReports.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpReprots.Collector.Http
{
    public class HttpCollectorMiddleware
    {
        private readonly RequestDelegate _next;

        public ILogger<HttpCollectorMiddleware> _logger;

        private IHttpReportsCollector _collector;

        public HttpCollectorMiddleware(RequestDelegate next, ILogger<HttpCollectorMiddleware> logger, IHttpReportsCollector collector)
        {
            _next = next;
            _logger = logger;
            _collector = collector;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            StringValues TransportType;

            if (!context.Request.Headers.TryGetValue(BasicConfig.TransportType, out TransportType))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;

                return;
            } 

            using (var Reader = new StreamReader(context.Request.Body))
            {
                var body = await Reader.ReadToEndAsync();

                if (!body.IsEmpty())
                {
                    await Save(HttpUtility.HtmlDecode(body),TransportType.ToString());
                } 
            }

            await context.Response.WriteAsync("Collect Success"); 
        }


        private async Task Save(string Body,string TransportType)
        {
            if (TransportType == typeof(RequestBag).Name)
            {
                try
                {
                    var package = JsonConvert.DeserializeObject<List<RequestBagJson>>(Body);

                    if (package != null && package.Any())
                    {
                        List<RequestBag> bags = new List<RequestBag>();

                        foreach (var item in package)
                        {
                            bags.Add(new RequestBag(item.RequestInfo as RequestInfo,item.RequestDetail as RequestDetail));
                        } 

                        await _collector.WriteRequestBag(bags);
                    }
                }
                catch (Exception ex)
                {

                    throw;
                } 
            }

            if (TransportType == typeof(Performance).Name)
            {
                var package = JsonConvert.DeserializeObject<Performance>(Body) as Performance;

                await _collector.WritePerformance(package);
            }  
        }  
    }
}
