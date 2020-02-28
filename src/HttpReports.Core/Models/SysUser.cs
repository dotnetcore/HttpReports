using HttpReports.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class SysUser: ISysUser
    { 
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

    }
}
