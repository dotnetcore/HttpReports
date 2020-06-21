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
    public class HttpClientDiagnosticListener : IDiagnosticListener, IObserver<KeyValuePair<string, object>>
    {
        private ILogger<HttpClientDiagnosticListener> _logger;

        private HttpContext _context;

        public HttpClientDiagnosticListener(ILogger<HttpClientDiagnosticListener> logger)
        {
            _logger = logger;  
        }


        public string ListenerName => "HttpHandlerDiagnosticListener";

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            _context = ServiceContainer.Provider.GetRequiredService<IHttpContextAccessor>()?.HttpContext;

            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
            {
                var request = value.Value.GetType().GetProperty("Request").GetValue(value.Value) as System.Net.Http.HttpRequestMessage;

                request.Headers.Add(BasicConfig.HttpClientTraceId,System.Diagnostics.Activity.Current.Id);   
            } 

            if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
            {
                var request = value.Value.GetType().GetProperty("Response").GetValue(value.Value) as System.Net.Http.HttpRequestMessage;

                request.Headers.Add(BasicConfig.HttpClientTraceId, System.Diagnostics.Activity.Current.Id);
            }

            _logger.LogInformation(value.Key);   
        }
    }
}
