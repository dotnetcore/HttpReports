using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{

    [Table(Name = "RequestDetail")]
    [Index("idx_detail_id", "Id", true)] 
    public class DBRequestDetail
    {
        [Column(IsPrimary = true, StringLength = 50)]
        public string Id { get; set; }

        [Column(StringLength = 50)]
        public string RequestId { get; set; }


        [Column(StringLength = 10)]
        public string Scheme { get; set; }

        [Column(DbType = "text")]
        public string QueryString { get; set; }

        [Column(DbType = "text")]
        public string Header { get; set; }

        [Column(DbType = "text")]
        public string Cookie { get; set; }

        [Column(DbType = "text")]
        public string RequestBody { get; set; }

        [Column(DbType = "text")]
        public string ResponseBody { get; set; }

        [Column(DbType = "text")]
        public string ErrorMessage { get; set; }

        [Column(DbType = "text")]
        public string ErrorStack { get; set; }

        [Column(DbType = "text")]
        public DateTime CreateTime { get; set; } 

    }
}
