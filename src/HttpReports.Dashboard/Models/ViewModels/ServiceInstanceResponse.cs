using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Dashboard.Models.ViewModels
{ 
    public class ServiceInstanceResponse
    {
        public string Service { get; set; }

        public List<string> Instance { get; set; }   

    }
}
