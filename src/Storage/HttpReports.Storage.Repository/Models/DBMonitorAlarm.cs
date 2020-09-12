using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{
    public class DBMonitorAlarm
    {
        [Table(Name = "MonitorAlarm")]
        [Index("idx_alarm_id", "Id", true)]
        public class DBMonitorJob
        {
            [Column(IsPrimary = true, StringLength = 50)]
            public string Id { get; set; }

            [Column(StringLength = 50)]
            public string JobId { get; set; }


            [Column(StringLength = 255)]
            public string Title { get; set; }


            [Column(StringLength = 50)]
            public string Service { get; set; }


            [Column(StringLength = 50)]
            public string Instance { get; set; }


            [Column(StringLength = 50)]
            public string Cronlike { get; set; }


            [Column(StringLength = 1000)]
            public string Body { get; set; } 


            public DateTime CreateTime { get; set; }   
 
           
        }
    } 

}
