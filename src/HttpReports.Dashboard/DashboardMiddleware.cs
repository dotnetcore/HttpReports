using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HttpReports.Dashboard.Handle;
using HttpReports.Dashboard.Route;
using System.Reflection;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Org.BouncyCastle.Asn1.X509.Qualified;
using HttpReports.Dashboard.Implements;

namespace HttpReports.Dashboard
{
    public class DashboardMiddleware
    {
        private readonly RequestDelegate _next;

        public DashboardMiddleware(RequestDelegate next)
        {
            _next = next;
        }


        public async Task InvokeAsync(HttpContext httpContext)
        {
            using var scope = httpContext.RequestServices.CreateScope();
            var options = scope.ServiceProvider.GetService<DashboardOptions>();

            var requestUrl = httpContext.Request.Path.Value;

            //EmbeddedFile 
            if (requestUrl.Contains("."))
            {
                await DashboardEmbeddedFiles.IncludeEmbeddedFile(httpContext, requestUrl);
                return;
            } 


            // Find Router
            var router = DashboardRoutes.Routes.FindRoute(requestUrl);

            if (router == null)
            {
                httpContext.Response.StatusCode = 404;
                return;
            }  

            var DashboardContext = new DashboardContext(httpContext, router, options);  

            //Activate Handle

            var handles = Assembly.GetAssembly(typeof(DashboardRoute)).GetTypes(); 

            var handleType = handles.FirstOrDefault(x => x.Name.Contains(router.Handle.Replace("HttpReports","Dashboard") + "Handle"));

            var handle = scope.ServiceProvider.GetRequiredService(handleType) as IDashboardHandle;

            if (handle == null)
            {
                httpContext.Response.StatusCode = 404;
                return;
            }


            //Authorization
            await AuthorizeHelper.AuthorizeAsync(httpContext,handle, router);  

            handle.Context = DashboardContext;

            string html;

            var method = handle.GetType().GetMethod(router.Action); 
            var parametersLength = method.GetParameters().Length;

            if (parametersLength == 0)
            {
                html = await (Task<string>)method.Invoke(handle, null);
            }
            else
            {
                if (httpContext.Request.ContentLength == null && httpContext.Request.Query.Count <= 0)
                {
                    html = await (Task<string>)method.Invoke(handle, new Object[] { null });
                }
                else
                {
                    object args;
                    if (httpContext.Request.Query.Count == 1)
                    {
                        var paraType = method.GetParameters().First().ParameterType;

                        if (paraType.Name.ToLowerInvariant().Contains("string"))
                        {
                            args = httpContext.Request.Query.FirstOrDefault().Value.ToString();
                        }
                        else
                        {
                            var dict = new Dictionary<string, string>();
                            httpContext.Request.Query.ToList().ForEach(x => dict.Add(x.Key, x.Value));
                            args = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dict));
                        } 
                    } 
                    else if (httpContext.Request.Query.Count > 1)
                    {
                        var dict = new Dictionary<string, string>();
                        httpContext.Request.Query.ToList().ForEach(x => dict.Add(x.Key, x.Value));
                        args = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(dict), method.GetParameters().First().ParameterType);
                    }
                    else
                    { 
                        string requestJson = await GetRequestBodyAsync(httpContext);

                        var paraType = method.GetParameters().First().ParameterType;

                        args = JsonConvert.DeserializeObject(requestJson, paraType);

                    }

                    html = await (Task<string>)method.Invoke(handle, new[] { args });

                }
            }

            await httpContext.Response.WriteAsync(html);
        }

        private async Task<string> GetRequestBodyAsync(HttpContext context)
        {
            try
            {
                string result = string.Empty;

                context.Request.EnableBuffering();

                var requestReader = new StreamReader(context.Request.Body, System.Text.Encoding.UTF8);

                result = await requestReader.ReadToEndAsync();

                context.Request.Body.Position = 0;

                return result;
            }
            catch (Exception ex)
            { 
                return string.Empty;
            }
        }
    }
}
