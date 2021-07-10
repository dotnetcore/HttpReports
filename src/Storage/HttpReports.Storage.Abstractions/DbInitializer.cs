using HttpReports.Core.Models; 
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Storage.Abstractions
{
    internal static  class DbInitializer
    {
        public static void Initialize(IFreeSql freeSql, FreeSql.DataType dataType)
        {
            var textType = dataType == FreeSql.DataType.SqlServer ? "nvarchar(max)" : "text";

            freeSql.CodeFirst.ConfigEntity<RequestInfo>(x => {

                x.Index("idx_info_id",nameof(RequestInfo.Id), true);
                x.Index("idx_info_service", nameof(RequestInfo.Service));
                x.Index("idx_info_instance", nameof(RequestInfo.Instance));
                x.Index("idx_info_milliseconds", nameof(RequestInfo.Milliseconds));
                x.Index("idx_info_statuscode", nameof(RequestInfo.StatusCode));
                x.Index("idx_info_createtime", nameof(RequestInfo.CreateTime));
                x.Index("idx_info_route", nameof(RequestInfo.Route)); 
                x.Index("idx_info_service_instance", "Service,Instance");
                x.Index("idx_info_instance_time", "Instance,CreateTime");
                x.Index("idx_info_id_route_statuscode_time", "Id,Route,StatusCode,CreateTime");
                x.Index("idx_info_service_parentservice_time", "Service,ParentService,CreateTime");
                x.Index("idx_info_service_instance_milliseconds_time", "Service,Instance,Milliseconds,CreateTime");
                x.Index("idx_info_service_instance_statuscode_time", "Service,Instance,StatusCode,CreateTime");

                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.Service).StringLength(50);
                x.Property(k => k.ParentService).StringLength(50);
                x.Property(k => k.Instance).StringLength(50);
                x.Property(k => k.Route).StringLength(100);
                x.Property(k => k.Url).StringLength(100);
                x.Property(k => k.RequestType).StringLength(10);
                x.Property(k => k.Method).StringLength(10);
                x.Property(k => k.RemoteIP).StringLength(50);
                x.Property(k => k.LoginUser).StringLength(50); 

            }); 

            freeSql.CodeFirst.ConfigEntity<RequestDetail>(x => {

                x.Index("idx_detail_id", nameof(RequestDetail.Id), true);
                x.Index("idx_detail_request_id", nameof(RequestDetail.RequestId)); 

                x.Property(k => k.Id).IsPrimary(true); 
                x.Property(k => k.Scheme).StringLength(10);  
                x.Property(k => k.QueryString).DbType(textType);
                x.Property(k => k.Header).DbType(textType);
                x.Property(k => k.Cookie).DbType(textType);
                x.Property(k => k.RequestBody).DbType(textType);
                x.Property(k => k.ResponseBody).DbType(textType);
                x.Property(k => k.ErrorMessage).DbType(textType);
                x.Property(k => k.ErrorStack).DbType(textType);

            });

            freeSql.CodeFirst.ConfigEntity<Performance>(x => {

                x.Index("idx_performance_id",nameof(Performance.Id), true);
                x.Index("idx_per_service", nameof(Performance.Service));
                x.Index("idx_per_instance", nameof(Performance.Instance));
                x.Index("idx_per_service_instance", "Service,Instance");

                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.Service).StringLength(50);
                x.Property(k => k.Instance).StringLength(50);

            });

            freeSql.CodeFirst.ConfigEntity<MonitorJob>(x => {

                x.Index("idx_job_id",nameof(MonitorJob.Id), true);
                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.Title).StringLength(255);
                x.Property(k => k.Description).StringLength(255);
                x.Property(k => k.CronLike).StringLength(50);
                x.Property(k => k.WebHook).StringLength(255);
                x.Property(k => k.Emails).StringLength(255);
                x.Property(k => k.Mobiles).StringLength(50);
                x.Property(k => k.Payload).DbType(textType);
                x.Property(k => k.Service).StringLength(50);
                x.Property(k => k.Instance).StringLength(50);
                x.Property(k => k.StartTime).StringLength(50);
                x.Property(k => k.EndTime).StringLength(50);

            });

            freeSql.CodeFirst.ConfigEntity<MonitorAlarm>(x => {

                x.Index("idx_alarm_id", nameof(MonitorAlarm.Id), true);
                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.JobId).StringLength(50);
                x.Property(k => k.Body).StringLength(1000); 

            });

            freeSql.CodeFirst.ConfigEntity<SysUser>(x => {

                x.Index("idx_user_id",nameof(SysUser.Id), true);
                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.UserName).StringLength(50);
                x.Property(k => k.Password).StringLength(50);

            });

            freeSql.CodeFirst.ConfigEntity<SysConfig>(x => {

                x.Index("idx_config_id", nameof(SysConfig.Id), true);
                x.Property(k => k.Id).IsPrimary(true);
                x.Property(k => k.Key).StringLength(50);
                x.Property(k => k.Value).StringLength(255);

            });


        }
    }
}
