using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HttpReports.Storage.Abstractions
{
    public class BaseStorageOptions:IOptions<BaseStorageOptions>
    {
        public string ConnectionString { get; set; }  
       
        public int DeferSecond { get; set; } 
     
        public int DeferThreshold { get; set; }


        public FreeSql.DataType DataType { get; set; }


        public BaseStorageOptions Value => this;


    }
}
