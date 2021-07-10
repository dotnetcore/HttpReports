using HttpReports.Core; 
using HttpReports.Core.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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

        private IHttpContextAccessor _httpContextAccessor;
      

        public HttpClientDiagnosticListener(ILogger<HttpClientDiagnosticListener> logger, IHttpContextAccessor httpContextAccessor, IReportsTransport transport, ISegmentContext context)
        {
            _logger = logger;
            _transport = transport;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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
            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
            {
                HandleHttpRequest(value.Value);
            }  
        } 

        private void HandleHttpRequest(object value)
        {
            try
            {
                HttpContext context = _httpContextAccessor.HttpContext;

                var request = value.GetType().GetProperty("Request").GetValue(value) as System.Net.Http.HttpRequestMessage;

                if (context != null && request != null && !request.RequestUri.ToString().Contains(BasicConfig.HttpCollectorEndpoint))
                {
                    if (!request.Headers.Contains(BasicConfig.ActiveTraceId) && context.Items.ContainsKey(BasicConfig.ActiveTraceId) )
                    {
                        var traceId = context.Items[BasicConfig.ActiveTraceId].ToString();

                        request.Headers.Add(BasicConfig.ActiveTraceId, traceId);
                    }

                    if (!request.Headers.Contains(BasicConfig.ActiveSpanService) && context.Items.ContainsKey(BasicConfig.ActiveSpanService))
                    {
                        var service = context.Items[BasicConfig.ActiveSpanService].ToString();

                        request.Headers.Add(BasicConfig.ActiveParentSpanService, service);
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("HandleHttpRequest:" + ex.ToString()); 
            } 
        }
      
    }
}
