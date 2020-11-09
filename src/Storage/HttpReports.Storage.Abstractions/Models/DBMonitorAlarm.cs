using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{ 
    [Table(Name = "MonitorAlarm")]
    [Index("idx_alarm_id", "Id", true)]
    public class DBMonitorAlarm
    {
        [Column(IsPrimary = true)]
        public long Id { get; set; }

        [Column(StringLength = 50)]
        public string JobId { get; set; } 
          
        [Column(StringLength = 1000)] 
        public string Body { get; set; }  

        public DateTime CreateTime { get; set; }   
 
           
    } 
}
