using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Diagnostics
{
    public interface IDiagnosticListener : IObserver<KeyValuePair<string, object>>
    {
        string ListenerName { get; } 

        IRequestChain Build(string Id);  

    }
}
