using HttpReports.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.ViewModels
{
    public class RequestInfoSearchResult
    {
        public List<RequestInfo> List { get; set; } = new List<RequestInfo>();

        public int Total { get; set; }
    }

 }
