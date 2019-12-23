using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public class HttpReportsOptions
    {  
        /// <summary>
        /// 默认为 api
        /// </summary>
        public string ApiPoint { get; set; } = "api";  

        /// <summary>
        /// 服务节点名称
        /// </summary>
        public string Node { get; set; }  

        /// <summary>
        ///  数据库类型 默认SqlServer,可选SqlServer,MySql
        /// </summary>
        public DBType DBType { get; set; }  

        /// <summary>
        ///  Web类型
        /// </summary>
        public WebType WebType { get; set; }  

    }

    public enum DBType
    { 
        SqlServer,
        MySql 
    }

    public enum WebType
    {    
        API,MVC
    } 

}
