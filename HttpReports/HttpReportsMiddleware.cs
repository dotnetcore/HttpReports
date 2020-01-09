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

        private IConfiguration _Configuration;

        public HttpReportsMiddleware(RequestDelegate next, IHttpReports httpReports,IConfiguration configuration)
        {
            this._next = next;
            this._httpReports = httpReports;
            this._Configuration = configuration;

            this._httpReports.Init(configuration);

        }

        public async Task InvokeAsync(HttpContext context)
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();

            await _next(context);

            stopwatch.Stop(); 
            
            _httpReports.Invoke(context, stopwatch.Elapsed.TotalMilliseconds, _Configuration); 
        } 
    }

}
