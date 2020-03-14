using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core.Config;
using HttpReports.Dashboard.Implements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HttpReports.Dashboard.Controllers
{
    public class HttpReportsController : Controller
    {
        private readonly IHttpReportsStorage _storage;

        public HttpReportsController(IHttpReportsStorage storage)
        {
            _storage = storage;
        }

        public async Task<IActionResult> Index()
        {
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public async Task<IActionResult> Trend()
        {
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        } 

        public async Task<IActionResult> EditMonitor(string Id = "")
        { 
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            ViewBag.Id = Id;

            return View();
        }


        public async Task<IActionResult> Monitor()
        {  
            var list = await _storage.GetMonitorJobs().ConfigureAwait(false);

            list.ForEach(x=>x.CronLike = ParseJobCronString(x.CronLike) ); 

            ViewBag.list = list;

            return View();
        } 
        public async Task<IActionResult> Detail()
        {
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public async Task<IActionResult> RequestInfoDetail(string Id = "")
        {
            var (requestInfo, requestDetail) = await _storage.GetRequestInfoDetail(Id); 

            ViewBag.Info = requestInfo;
            ViewBag.Detail = requestDetail;  

            return View();  
        }


        private string ParseJobCronString(string cron)
        {
            if (cron == "0 0/1 * * * ?") return "1分钟";
            if (cron == "0 0/3 * * * ?") return "3分钟";
            if (cron == "0 0/5 * * * ?") return "5分钟";
            if (cron == "0 0/10 * * * ?") return "10分钟";
            if (cron == "0 0/30 * * * ?") return "30分钟";
            if (cron == "0 0 0/1 * * ?") return "1小时";
            if (cron == "0 0 0/2 * * ?") return "2小时";
            if (cron == "0 0 0/4 * * ?") return "4小时";
            if (cron == "0 0 0/6 * * ?") return "6小时";
            if (cron == "0 0 0/8 * * ?") return "8小时";
            if (cron == "0 0 0/12 * * ?") return "12小时";
            if (cron == "0 0 0 1/1 * ?") return "1天";

            return "1分钟";
        }


        [AllowAnonymous]
        public IActionResult UserLogin()
        {
            return View(); 
        }


        [AllowAnonymous]
        public IActionResult UserLogout()
        {
            HttpContext.DeleteCookie(BasicConfig.LoginCookieId);

            return new RedirectResult("/HttpReports/UserLogin");
        } 

    }
}