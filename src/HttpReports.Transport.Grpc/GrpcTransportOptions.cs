using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Transport.Grpc
{
    public class GrpcTransportOptions : IOptions<GrpcTransportOptions>
    {
        public Uri CollectorAddress { get; set; }   

        public int DeferSecond { get; set; }

        public int DeferThreshold { get; set; }    

        public GrpcTransportOptions Value => this;

    }
}
