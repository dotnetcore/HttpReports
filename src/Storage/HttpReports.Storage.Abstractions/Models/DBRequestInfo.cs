using FreeSql.DataAnnotations;
using FreeSql.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions.Models
{ 
    [Table(Name = "RequestInfo" )] 

    [Index("idx_info_id", "Id", true)]
    [Index("idx_info_service", "Service")] 
    [Index("idx_info_instance", "Instance")]
    [Index("idx_info_milliseconds", "Milliseconds")]
    [Index("idx_info_statuscode", "StatusCode")] 
    [Index("idx_info_createtime", "CreateTime")]
    [Index("idx_info_route", "Route")]

    [Index("idx_info_service_instance", "Service,Instance")]
    [Index("idx_info_instance_time", "Instance,CreateTime")]
    [Index("idx_info_id_route_statuscode_time", "Id,Route,StatusCode,CreateTime")]
    [Index("idx_info_service_parentservice_time", "Service,ParentService,CreateTime")]
    [Index("idx_info_service_instance_milliseconds_time", "Service,Instance,Milliseconds,CreateTime")]  
    [Index("idx_info_service_instance_statuscode_time", "Service,Instance,StatusCode,CreateTime")]  

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

        [Column(StringLength = 100)] 
        public string Route { get; set; }


        [Column(StringLength = 100)] 
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
