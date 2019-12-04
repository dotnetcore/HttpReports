using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports
{
    public class HttpReportsOptions
    { 
        public string APiPoint { get; set; } = "api";


        /// <summary>
        ///  数据库类型 默认SqlServer,可选SqlServer,MySql
        /// </summary>
        public DBType DBType { get; set; } = DBType.SqlServer;

    }

    public enum DBType
    { 
        SqlServer,
        MySql 
    } 
}
