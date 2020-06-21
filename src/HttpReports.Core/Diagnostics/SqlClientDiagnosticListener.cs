using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Diagnostics
{
    public class SqlClientDiagnosticListener : IDiagnosticListener
    {
        public string ListenerName => "SqlClientDiagnosticListener";

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
            if (value.Key == "System.Data.SqlClient.WriteCommandBefore")
            {
                //
            }

            if (value.Key == "System.Data.SqlClient.WriteCommandAfter")
            {
                //
            }

            if (value.Key == "System.Data.SqlClient.WriteCommandError")
            {

            }  
        }
    }
}
