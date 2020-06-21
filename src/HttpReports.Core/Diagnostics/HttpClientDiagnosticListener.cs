using AspectCore.Extensions.Reflection;
using HttpReports.Core.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Diagnostics
{
    public class HttpClientDiagnosticListener : IDiagnosticListener 
    {
        private ILogger<HttpClientDiagnosticListener> _logger;

        private HttpContext _context;

        private IReportsTransport _transport;

        public HttpClientDiagnosticListener(ILogger<HttpClientDiagnosticListener> logger,IReportsTransport transport)
        {
            _logger = logger;
            _transport = transport;
        }


        public string ListenerName => "HttpHandlerDiagnosticListener";

        public void OnCompleted()
        {
             
        }

        public void OnError(Exception error)
        {
             
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            _context = ServiceContainer.Provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
            {
                var a = System.Diagnostics.Activity.Current;

                _ = a.TraceId;
                _ = a.ParentSpanId;
                _ = a.SpanId; 

                var request = value.Value.GetType().GetProperty("Request").GetValue(value.Value) as System.Net.Http.HttpRequestMessage; 

                //request.Headers.Add(BasicConfig.ActiveTraceId,_context.GetTraceId()); 
            } 

            if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
            {
                var a = System.Diagnostics.Activity.Current; 

                var request = value.Value.GetType().GetProperty("Response").GetValue(value.Value) as System.Net.Http.HttpRequestMessage;  
            }

            _logger.LogInformation(value.Key);   
        }
    }
}
