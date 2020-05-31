using HttpReports.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Service
{
    public interface IPerformanceService
    {
        Task<IPerformance> GetPerformance(string Instance);  
    }
}
