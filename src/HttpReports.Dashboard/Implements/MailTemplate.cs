using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Implements
{
    public static class MailTemplate
    {
        public static string RT { get; set; } = @"

          <p>【响应超时】触发预警 </p> 

          <p> 超时率预警值：value  当前：now </p> 
                  
         ";  

    }
}
