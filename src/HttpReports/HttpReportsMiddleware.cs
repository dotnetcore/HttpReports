using System;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HttpReports
{
    public class HttpReportsMiddleware
    {
        private RequestDelegate _next;

        private Stopwatch stopwatch;

        private IHttpReports _httpReports;

        private IConfiguration _Configuration;

        public HttpReportsMiddleware(RequestDelegate next, IHttpReports httpReports, IConfiguration configuration)
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

            await Execute(context);

            stopwatch.Stop();
            _httpReports.Invoke(context, stopwatch.Elapsed.TotalMilliseconds, _Configuration);
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