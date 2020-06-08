using HttpReports.Core.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Models
{
    public class SysConfig : ISysConfig
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
