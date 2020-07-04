using HttpReports.Core;
using HttpReports.Core.Config;
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

                _context.Push(activity?.SpanId.ToHexString(), new Segment
                {

                    activity = activity,
                    CreateTime = DateTime.Now,
                    Value = request
                });

                if (!request.Headers.TryGetValues(BasicConfig.HttpClientTraceId, out _))
                {
                    var ActiveTraceId = activity.GetBaggageItem(BasicConfig.ActiveTraceId);

                    request.Headers.Add(BasicConfig.HttpClientTraceId, ActiveTraceId);
                }
            }

            if (value.Key == "System.Net.Http.HttpRequestOut.Stop")
            {
                var response = value.Value.GetType().GetProperty("Response").GetValue(value.Value) as System.Net.Http.HttpResponseMessage;

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

            if (Segments.Count != 2 || Segments[0] == null || Segments[1] == null)
            {
                return null;
            }

            IRequestChain requestChain = new RequestChain();

            _context.Release(Id);

            return requestChain;
        }
    }
}
