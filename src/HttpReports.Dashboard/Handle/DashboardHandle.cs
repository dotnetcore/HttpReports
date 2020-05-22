using HttpReports.Core.Config;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Handle
{
    public class DashboardHandle : DashboardHandleBase
    {
        private readonly IHttpReportsStorage _storage;
        private readonly IOptions<DashboardOptions> _options;

        private readonly LocalizeService _localizeService;
        private Localize _lang;


        public DashboardHandle(IServiceProvider serviceProvider, IHttpReportsStorage storage, IOptions<DashboardOptions> options, LocalizeService localizeService) : base(serviceProvider)
        {
            _storage = storage;
            _options = options;
            _localizeService = localizeService; 
            _lang = _localizeService.Current;
        }


        public async Task<string> Index()
        {
            ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewData["nodes"] = nodes;

            return await View();
        } 

        public async Task<string> Trend()
        {
            ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewData["nodes"] = nodes;

            return await View();
        }

        public async Task<string> EditMonitor(string Id = "")
        {
            ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewData["nodes"] = nodes;

            ViewData["Id"] = Id;

            return await View();
        }

        public async Task<string> Monitor()
        {
            ConfigLanguage();

            var list = await _storage.GetMonitorJobs();

            list.ForEach(x => x.CronLike = ParseJobCronString(x.CronLike));

            ViewData["list"] = list;

            return await View();
        }
        public async Task<string> Detail()
        {
            ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewData["nodes"] = nodes;

            return await View();
        }

        public async Task<string> Trace(string Id)
        {
            ConfigLanguage();

            ViewData["TraceId"] = Id;

            return await View();
        }

        public async Task<string> RequestInfoDetail(string Id = "")
        {
            ConfigLanguage();

            var (requestInfo, requestDetail) = await _storage.GetRequestInfoDetail(Id);

            ViewData["Info"] = requestInfo;
            ViewData["Detail"] = requestDetail;

            return await View();
        }

        private string ParseJobCronString(string cron)
        { 
            if (cron == "0 0/1 * * * ?") return _lang.Monitor_Time1Min;
            if (cron == "0 0/3 * * * ?") return _lang.Monitor_Time3Min;
            if (cron == "0 0/5 * * * ?") return _lang.Monitor_Time5Min;
            if (cron == "0 0/10 * * * ?") return _lang.Monitor_Time10Min;
            if (cron == "0 0/30 * * * ?") return _lang.Monitor_Time30Min;
            if (cron == "0 0 0/1 * * ?") return _lang.Monitor_Time1Hour;
            if (cron == "0 0 0/2 * * ?") return _lang.Monitor_Time2Hour;
            if (cron == "0 0 0/4 * * ?") return _lang.Monitor_Time4Hour;
            if (cron == "0 0 0/6 * * ?") return _lang.Monitor_Time6Hour;
            if (cron == "0 0 0/8 * * ?") return _lang.Monitor_Time8Hour;
            if (cron == "0 0 0/12 * * ?") return _lang.Monitor_Time12Hour;
            if (cron == "0 0 0 1/1 * ?") return _lang.Monitor_Time1Day;

            return _lang.Monitor_Time1Min;
        }

        private void ConfigLanguage()
        { 
            ViewData["Language"] = _lang;
        }


        [AllowAnonymous]
        public async Task<string> UserLogin()
        {
            ConfigLanguage();

            if (_options.Value.AllowAnonymous)
            {
               Context.HttpContext.SetCookie(BasicConfig.LoginCookieId, "admin", 60 * 30 * 7);
               Context.HttpContext.Response.Redirect("/HttpReports");
                return string.Empty;
            }

            return await View();
        }


        [AllowAnonymous]
        public Task<string> UserLogout()
        {
           Context.HttpContext.DeleteCookie(BasicConfig.LoginCookieId);

           Context.HttpContext.Response.Redirect("/HttpReports/UserLogin");

           return Task.FromResult(string.Empty);
        } 

    }
}
