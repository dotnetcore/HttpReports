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

        private IReportsTransport _transport;

        private ISegmentContext _context;


        public HttpClientDiagnosticListener(ILogger<HttpClientDiagnosticListener> logger,IReportsTransport transport, ISegmentContext context)
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
            if (value.Key == "System.Net.Http.HttpRequestOut.Start")
            {
                var activity = System.Diagnostics.Activity.Current; 
                
                var request = value.Value.GetType().GetProperty("Request").GetValue(value.Value) as System.Net.Http.HttpRequestMessage; 

                _context.Push(activity?.SpanId.ToHexString(),new Segment { 
                
                    activity = activity,
                    CreateTime = DateTime.Now,
                    Value = request
                
                }); 
              
            } 

            if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
            {
                var activity = System.Diagnostics.Activity.Current; 

                var response = value.Value.GetType().GetProperty("Response").GetValue(value.Value) as System.Net.Http.HttpRequestMessage;

                _context.Push(activity?.SpanId.ToHexString(), new Segment
                { 
                    activity = activity,
                    CreateTime = DateTime.Now,
                    Value = response 
                }); 

                Build(activity?.SpanId.ToHexString()); 
            }

            _logger.LogInformation(value.Key);   
        }

        public IRequestChain Build(string Id)
        { 
            var Segments = _context.GetSegments(Id); 

            if (Segments.Count != 2 ||  Segments[0] == null || Segments[1] == null)
            {
                return null;
            }  
            

            _context.Release(Id);

            return new RequestChain();
        }
    }
}
