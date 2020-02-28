using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.ViewModels
{
    public class UpdateAccountRequest
    {
        public string Username { get; set; }

        public string NewUserName { get; set; }

        public string OldPwd { get; set; }

        public string NewPwd { get; set; } 
    }
}
