using HttpReports.Core;
using HttpReports.Core.Diagnostics;
using HttpReports.Storage.Abstractions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HttpReports.Diagnostic.MySqlConnector
{
    public class MySqlConnectorDiagnosticListener : DefaultTraceListener, IDiagnosticListener
    {
        private ILogger<MySqlConnectorDiagnosticListener> _logger;

        private IReportsTransport _transport;

        private ISegmentContext _context;

        private IHttpReportsStorage _storage;


        public MySqlConnectorDiagnosticListener(ILogger<MySqlConnectorDiagnosticListener> logger, IReportsTransport transport, ISegmentContext context)
        {
            _logger = logger;
            _transport = transport;
            _context = context; 

        }

        public string ListenerName => "MySqlDiagnosticListener";

        public IRequestChain Build(string Id)
        {
            return null;
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
           
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(KeyValuePair<string, object> value)
        {

        }
    }
}
