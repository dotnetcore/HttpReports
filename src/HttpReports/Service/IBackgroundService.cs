using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpReports.Service
{
    public interface IBackgroundService
    { 
        Task StartAsync(IApplicationBuilder builder,CancellationToken Token = default);

        Task ExecuteAsync(CancellationToken Token = default); 
       
    }
}
