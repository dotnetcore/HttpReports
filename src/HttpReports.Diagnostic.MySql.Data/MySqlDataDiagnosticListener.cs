using HttpReports.Core;
using HttpReports.Core.Diagnostics;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace HttpReports.Diagnostic.MySql.Data
{
    public class MySqlDataDiagnosticListener : DefaultTraceListener, IDiagnosticListener
    {
        private ILogger<MySqlDataDiagnosticListener> _logger;

        private IReportsTransport _transport;

        private ISegmentContext _context;

        public MySqlDataDiagnosticListener(ILogger<MySqlDataDiagnosticListener> logger, IReportsTransport transport, ISegmentContext context)
        {
            _logger = logger;
            _transport = transport;
            _context = context;

            MySqlTrace.Listeners.Clear();
            MySqlTrace.Listeners.Add(this);
            MySqlTrace.Switch.Level = SourceLevels.Information;
            MySqlTrace.QueryAnalysisEnabled = true;

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
