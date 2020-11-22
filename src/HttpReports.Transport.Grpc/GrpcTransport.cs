using HttpReports.Core;
using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Transport.Grpc
{
    public class GrpcTransport : IReportsTransport
    {
        public Task SendDataAsync(RequestBag bag)
        {
            throw new NotImplementedException();
        }

        public Task SendDataAsync(Performance performance)
        {
            throw new NotImplementedException();
        }
    }
}
