using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.PostgreSQL.Models
{

    [Table(Name = "MonitorJob")]
    [Index("idx_job_id", "Id", true)]
    public class DBMonitorJob
    {
        [Column(IsPrimary = true, StringLength = 50)]
        public string Id { get; set; }


        [Column(StringLength = 255)]
        public string Title { get; set; }


        [Column(StringLength = 255)] 
        public string Description { get; set; }


        [Column(StringLength = 50)] 
        public string CronLike { get; set; }

        [Column(StringLength = 255)] 
        public string WebHook { get; set; }


        [Column(StringLength = 255)]
        public string Emails { get; set; }


        [Column(StringLength = 50)]
        public string Mobiles { get; set; }

        public int Status { get; set; }


        [Column(StringLength = 50)]
        public string Service { get; set; }


        [Column(StringLength = 50)]
        public string Instance { get; set; }

        [Column(DbType = "text")]
        public string Payload { get; set; }


        public DateTime CreateTime { get; set; }

    }
}
