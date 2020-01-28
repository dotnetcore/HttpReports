using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Monitor
{
    public interface IMonitorJob
    {
         int Id { get; set; } 

        string Title { get; set; }

         string Description { get; set; }

         string CronLike { get; set; }

         string Emails { get; set; }

         string Mobiles { get; set; }

         int Status { get; set; }

         string Nodes { get; set; }

         string Payload { get; set; }

         DateTime CreateTime { get; set; }
    }
}
