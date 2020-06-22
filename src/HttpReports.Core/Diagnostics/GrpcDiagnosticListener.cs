using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Diagnostics
{
    public class GrpcDiagnosticListener : IDiagnosticListener
    {
        public string ListenerName => "Grpc.Net.Client";

        public IRequestChain Build(string Id)
        {
            return null;
        }

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
            var current = System.Diagnostics.Activity.Current;

            if (value.Key == "Grpc.Net.Client.GrpcOut.Start")
            {
                var cc = value.Value.GetType();  

                var request = value.Value.GetType().GetProperty("Request").GetValue(value.Value) as System.Net.Http.HttpRequestMessage;
                 

            }

            if (value.Key == "Grpc.Net.Client.GrpcOut.Stop")
            {

            } 
        }
    }
}
