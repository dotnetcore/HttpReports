using System;
using System.Net.Http;

using Grpc.Net.Client;

using Microsoft.Extensions.Options;

namespace HttpReports.DataWriter.Grpc
{
    public class GrpcReportsDataWriterOptions : IOptions<GrpcReportsDataWriterOptions>
    {
        public Uri CollectorAddress { get; set; }

        public bool AllowAnyRemoteCertificate { get; set; } = false;

        public Func<HttpClient> NeedHttpClientFunc = null;

        public Action<GrpcChannelOptions> PostConfigGrpcChannelOptions = null;

        public GrpcReportsDataWriterOptions Value => this;
    }
}