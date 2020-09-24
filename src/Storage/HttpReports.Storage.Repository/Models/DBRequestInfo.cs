using FreeSql.DataAnnotations;
using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{ 
    [Table(Name = "RequestInfo" )] 
    //[Index("idx_info_id", "Id", true)]
    //[Index("idx_info_createtime", "CreateTime")]
    //[Index("idx_info_service", "Service")]
    //[Index("idx_info_service_parentservice", "Service,ParentService")]
    //[Index("idx_info_instance", "Instance")]
    //[Index("idx_info_service_instance", "Service,Instance")] 
    public class DBRequestInfo
    { 
        [Column(IsPrimary = true,StringLength = 50)]
        public string Id { get; set; } 

        [Column(StringLength = 50)]
        public string ParentId { get; set; }

        [Column(StringLength = 50)]
        public string Service { get; set; }


        [Column(StringLength = 50)]
        public string ParentService { get; set; }

        [Column(StringLength = 50)]  
        public string Instance { get; set; }

        [Column(StringLength = 200)] 
        public string Route { get; set; }


        [Column(StringLength = 200)] 
        public string Url { get; set; }


        [Column(StringLength = 10)]
        public string RequestType { get; set; }


        [Column(StringLength = 10)]
        public string Method { get; set; }

        public int Milliseconds { get; set; }

        public int StatusCode { get; set; }


        [Column(StringLength = 50)]
        public string RemoteIP { get; set; }


        [Column(StringLength = 50)]
        public string LoginUser { get; set; }


        public DateTime CreateTime { get; set; } 

    }

}
