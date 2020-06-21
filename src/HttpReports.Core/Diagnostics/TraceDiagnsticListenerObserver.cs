using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HttpReports.Core.Diagnostics
{
    public class TraceDiagnsticListenerObserver : IObserver<DiagnosticListener>
    {
        private IEnumerable<IDiagnosticListener> _listeners;

        public TraceDiagnsticListenerObserver(IEnumerable<IDiagnosticListener> listeners)
        {
            _listeners = listeners;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {

        }

        public void OnNext(DiagnosticListener listener)
        {
            IDiagnosticListener diagnosticListener = _listeners.Where(x => x.ListenerName == listener.Name).FirstOrDefault();

            if (diagnosticListener != null)
            {
                listener.Subscribe(diagnosticListener);  
            }
        }
    }
}
