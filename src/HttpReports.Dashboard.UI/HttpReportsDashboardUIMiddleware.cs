using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

#if NETSTANDARD2_0

using Microsoft.AspNetCore.Hosting;

#elif NETCOREAPP3_1

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

#endif

namespace HttpReports.Dashboard
{
    internal class HttpReportsDashboardUIMiddleware
    {
        private const string EmbeddedFileNamespace = "HttpReports.Dashboard.web";

        private readonly StaticFileMiddleware _staticFileMiddleware;
        private readonly DashboardUIOptions _options;
        private readonly JsonSerializerSettings _serializerSettings;

        public HttpReportsDashboardUIMiddleware(RequestDelegate next,
                                                IOptions<DashboardUIOptions> options,
                                                IHostingEnvironment hostingEnv,
                                                ILoggerFactory loggerFactory)
        {
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            _staticFileMiddleware = CreateStaticFileMiddleware(next, hostingEnv, loggerFactory);

            _serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (string.Equals(httpContext.Request.Method, "GET")
                && httpContext.Request.Path.StartsWithSegments(_options.RoutePrefix))
            {
                var path = httpContext.Request.Path.Value;
                if (path.Equals(_options.RoutePrefix, StringComparison.OrdinalIgnoreCase)
                    || (path[path.Length - 1] == '/' && path.Length == _options.RoutePrefix.Length + 1))
                {
                    httpContext.Response.StatusCode = 302;
                    httpContext.Response.Headers["Location"] = $"{_options.RoutePrefix}/index.html";

                    return;
                }
                if (path.EndsWith("config.json", StringComparison.OrdinalIgnoreCase))
                {
                    //获取配置
                    var config = JsonConvert.SerializeObject(new UIConfigDto()
                    {
                        ApiAddress = _options.APIAddress,
                    }, _serializerSettings);
#if DEBUG
                    httpContext.Response.Headers.Add("Access-Control-Allow-Origin", "*");
#endif
                    httpContext.Response.ContentType = "text/json;charset=utf-8";
                    await httpContext.Response.WriteAsync(config, Encoding.UTF8);
                    return;
                }
            }
            await _staticFileMiddleware.Invoke(httpContext);
        }

        private StaticFileMiddleware CreateStaticFileMiddleware(RequestDelegate next,
                                                                IHostingEnvironment hostingEnv,
                                                                ILoggerFactory loggerFactory)
        {
            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = string.IsNullOrEmpty(_options.RoutePrefix) ? string.Empty : _options.RoutePrefix,
                FileProvider = new EmbeddedFileProvider(typeof(HttpReportsDashboardUIMiddleware).Assembly, EmbeddedFileNamespace),
            };

            return new StaticFileMiddleware(next, hostingEnv, Options.Create(staticFileOptions), loggerFactory);
        }
    }
}