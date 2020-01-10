using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Models
{

    [Table("Job")]
    public class Job
    {

        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// 任务标题
        /// </summary>
        public string Title { get; set; }


        /// <summary>
        /// CronLike 表达式
        /// </summary>
        public string CronLike { get; set; }


        /// <summary>
        /// 邮箱列表
        /// </summary>
        public string Emails { get; set; }


        /// <summary>
        /// 手机号列表
        /// </summary>
        public string Mobiles { get; set; }


        /// <summary>
        /// 任务状态 1开始 0关闭
        /// </summary>
        public int Status { get; set; } 

        /// <summary>
        /// 服务节点
        /// </summary>
        public string Servers { get; set; }


        /// <summary>
        /// 响应超时配置 1开启0关闭
        /// </summary>
        public int RtStatus { get; set; }

        /// <summary>
        /// 响应超时时间 ms
        /// </summary>
        public int RtTime { get; set; } 

        /// <summary>
        /// 响应超时百分比
        /// </summary>
        public double RtRate { get; set; }


        /// <summary>
        /// 请求状态码配置 1开启0关闭
        /// </summary>
        public int HttpStatus { get; set; }


        /// <summary>
        /// Http 状态码列表
        /// </summary>
        public string HttpCodes { get; set; }


        /// <summary>
        /// HTTP 状态码 百分比
        /// </summary>
        public double HttpRate { get; set; }


        /// <summary>
        /// IP监控开关 1开启0关闭
        /// </summary>
        public int IPStatus { get; set; }


        /// <summary>
        /// IP监控 白名单
        /// </summary>
        public string IPWhiteList { get; set; }


        /// <summary>
        /// IP监控百分比
        /// </summary>
        public double IPRate { get; set; }


        /// <summary>
        /// 单位时间请求量 监控开关 1开 0关
        /// </summary>
        public int RequestStatus { get; set; }


        /// <summary>
        /// 单位时间最大请求量
        /// </summary>
        public int RequestCount { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }   

    }
}
