using HttpReports.Core; 
using HttpReports.Core.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Diagnostic.HttpClient
{
    public class HttpClientDiagnosticListener : IDiagnosticListener
    {
        private ILogger<HttpClientDiagnosticListener> _logger;

        private IReportsTransport _transport;

        private ISegmentContext _context;  

        public HttpClientDiagnosticListener(ILogger<HttpClientDiagnosticListener> logger, IReportsTransport transport, ISegmentContext context)
        {
            _logger = logger;
            _transport = transport;
            _context = context;
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
             var activity = System.Diagnostics.Activity.Current;  

            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
            {
                var request = value.Value.GetType().GetProperty("Request").GetValue(value.Value) as System.Net.Http.HttpRequestMessage; 

                if (!request.Headers.TryGetValues(BasicConfig.ActiveTraceId, out _))
                {
                    var TraceId = activity.GetBaggageItem(BasicConfig.ActiveTraceId); 

                    request.Headers.Add(BasicConfig.ActiveTraceId, TraceId);
                }

                if (!request.Headers.TryGetValues(BasicConfig.ActiveSpanService, out _))
                {
                    var Service = activity.GetBaggageItem(BasicConfig.ActiveSpanService);

                    request.Headers.Add(BasicConfig.ActiveParentSpanService, Service);
                } 
                
            }  
        } 
      
    }
}
