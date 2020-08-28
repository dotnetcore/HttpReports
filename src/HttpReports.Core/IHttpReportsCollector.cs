using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Core
{
    public interface IHttpReportsCollector
    {
        Task WriteRequestBag(List<RequestBag> list);

        Task WritePerformance(Performance performance);

    }
}
