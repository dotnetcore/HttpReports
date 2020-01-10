using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Models
{
    
    public class JobRequest
    { 
        public int Id { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Title { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CronLike { get; set; } 
        public int Rate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Email { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Mobiles { get; set; }

        public int Status { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Node { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public int RtStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RtTime { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RtRate { get; set; } 

        public int HttpStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HttpCodes { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string HttpRate { get; set; } 

        public int IPStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IPWhiteList { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string IPRate { get; set; } 

        public int RequestStatus { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RequestCount { get; set; } 

    }
}
