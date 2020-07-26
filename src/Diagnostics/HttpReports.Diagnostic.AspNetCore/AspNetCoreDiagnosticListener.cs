using HttpReports.Core;
using HttpReports.Core.Config;
using HttpReports.Core.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HttpReports.Diagnostic.AspNetCore
{
    public class AspNetCoreDiagnosticListener : IDiagnosticListener
    { 
        private ILogger<AspNetCoreDiagnosticListener> _logger;

        private IReportsTransport _transport;

        private ISegmentContext _context;
        private IHttpContextAccessor _httpContextAccessor;


        public AspNetCoreDiagnosticListener(ILogger<AspNetCoreDiagnosticListener> logger, IReportsTransport transport, ISegmentContext context,IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _transport = transport;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            
        }


        public string ListenerName => "Microsoft.AspNetCore";

        public void OnCompleted()
        {
             
        }

        public void OnError(Exception error)
        {
           
        }

        public void OnNext(KeyValuePair<string, object> value)
        { 
            if(value.Key == "Microsoft.AspNetCore.Diagnostics.UnhandledException")
            {
                HttpContext context = _httpContextAccessor.HttpContext;

                Exception exception = value.Value.GetType().GetProperty("exception").GetValue(value.Value) as Exception;

                if (context != null && exception != null)
                {
                    context.Items.Add(BasicConfig.HttpReportsGlobalException,exception);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                } 
            } 
        }
    }
}
