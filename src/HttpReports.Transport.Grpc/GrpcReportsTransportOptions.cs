using System;
using System.Net.Http;

using Grpc.Net.Client;

using Microsoft.Extensions.Options;

namespace HttpReports.Transport.Grpc
{
    public class GrpcReportsTransportOptions : IOptions<GrpcReportsTransportOptions>
    {
        public Uri CollectorAddress { get; set; }

        public bool AllowAnyRemoteCertificate { get; set; } = false;

        public Func<HttpClient> NeedHttpClientFunc = null;

        public Action<GrpcChannelOptions> PostConfigGrpcChannelOptions = null;

        public GrpcReportsTransportOptions Value => this;
    }
}