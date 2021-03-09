using HttpReports.Core;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Handles
{
    public class DashboardEmbeddedFiles
    {
        static readonly Dictionary<string, string> ResponseType = new Dictionary<string, string>
        {
            { ".css","text/css"},
            { ".html","text/html"},
            { ".ico","image/x-icon"},
            { ".png","image/png"},
            { ".jpg","image/jpg"},
            { ".js","application/javascript"},
            { ".json","application/json;charset=utf-8"},
            {".woff2","font/woff2" },
            {".woff","font/woff" },
            {".ttf","application/octet-stream" },
            {".map","application/octet-stream" }
        };

        private static Assembly _assembly;

        static DashboardEmbeddedFiles()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        public static async Task IncludeEmbeddedFile(HttpContext context, string path)
        {

            context.Response.OnStarting(() =>
            {
                if (context.Response.StatusCode == (int)HttpStatusCode.OK)
                {
                   context.Response.ContentType = ResponseType[Path.GetExtension(path)];
                }

                return Task.CompletedTask;
            });

            path = BasicConfig.StaticFilesRoot + path.Replace("/",".");

            var list = _assembly.GetManifestResourceNames().ToList();

            using (var inputStream = _assembly.GetManifestResourceStream(path))
            {
                if (inputStream == null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }
                await inputStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
            }
        }
    }
}
