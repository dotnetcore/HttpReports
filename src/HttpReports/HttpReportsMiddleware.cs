using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports
{
    public class HttpReportsMiddleware
    {
        private RequestDelegate _next;

        private Stopwatch stopwatch;

        private IHttpReports _httpReports;

        public  static IConfiguration Configuration;

        public HttpReportsMiddleware(RequestDelegate next, IHttpReports httpReports)
        {
            this._next = next;
            this._httpReports = httpReports;
            this._httpReports.Init(Configuration);

        }

        public async Task InvokeAsync(HttpContext context)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start(); 

            await Execute(context);  

            stopwatch.Stop(); 
            _httpReports.Invoke(context, stopwatch.Elapsed.TotalMilliseconds, Configuration); 
        } 

        private async Task Execute(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
            }  
        }  

    } 
}
