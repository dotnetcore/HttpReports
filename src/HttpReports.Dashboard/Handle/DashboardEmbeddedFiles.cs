using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Handle
{
    public class DashboardEmbeddedFiles
    {
        static readonly Dictionary<string, string> ResponseType = new Dictionary<string, string>
        {
            { ".css","text/css"},
            { ".ico","image/x-icon"},
            { ".png","image/png"},
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

            using (var inputStream = _assembly.GetManifestResourceStream(path))
            {
                if (inputStream == null)
                {
                    throw new ArgumentException($@"Resource with name {path.Substring(1)} not found in assembly {_assembly}.");
                }
                await inputStream.CopyToAsync(context.Response.Body).ConfigureAwait(false);
            }
        }
    }
}
