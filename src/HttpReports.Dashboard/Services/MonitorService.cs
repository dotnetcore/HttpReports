
using HttpReports.Core.Models;
using HttpReports.Dashboard.ViewModels;
using HttpReports.Models;
using HttpReports.Monitor;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class MonitorService
    { 
        private readonly LocalizeService _localizeService;
        private Localize _lang => _localizeService.Current;

        public MonitorService(LocalizeService localizeService)
        {
            _localizeService = localizeService;
        }     

    }
}
