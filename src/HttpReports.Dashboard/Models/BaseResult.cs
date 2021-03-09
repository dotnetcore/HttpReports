using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models
{
    public class BaseResult
    {
        public int Code { get; set; }

        public string Msg { get; set; } 

        public object Data { get; set; }  
    }  

}  
