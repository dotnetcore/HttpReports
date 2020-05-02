using System.Linq;
using System.Threading.Tasks;
using HttpReports.Core.Config;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Services.Language;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HttpReports.Dashboard.Controllers
{
    public class HttpReportsController : Controller
    {
        private readonly IHttpReportsStorage _storage;
        private readonly IOptions<DashboardOptions> _options;
        private readonly LanguageService _language;

        public HttpReportsController(IHttpReportsStorage storage, IOptions<DashboardOptions> options, LanguageService language)
        {
            _storage = storage;
            _options = options;
            _language = language;
        }

        public async Task<IActionResult> Index()
        {
            await ConfigLanguage(); 

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public async Task<IActionResult> Trend()
        {
            await ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public async Task<IActionResult> EditMonitor(string Id = "")
        {
            await ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            ViewBag.Id = Id;

            return View();
        }


        public async Task<IActionResult> Monitor()
        {
            await ConfigLanguage();

            var list = await _storage.GetMonitorJobs();

            list.ForEach(x => x.CronLike = ParseJobCronString(x.CronLike));

            ViewBag.list = list;

            return View();
        }
        public async Task<IActionResult> Detail()
        {
            await ConfigLanguage();

            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public async Task<IActionResult> Trace(string Id)
        {
            await ConfigLanguage();

            ViewBag.TraceId = Id;

            return View();
        }

        public async Task<IActionResult> RequestInfoDetail(string Id = "")
        {
            await ConfigLanguage();

            var (requestInfo, requestDetail) = await _storage.GetRequestInfoDetail(Id);

            ViewBag.Info = requestInfo;
            ViewBag.Detail = requestDetail;

            return View();
        }

        private string ParseJobCronString(string cron)
        {
            ILanguage _lang =  _language.GetLanguage().Result;

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

        private async Task ConfigLanguage()
        {
            ILanguage language = await _language.GetLanguage(); 

            ViewBag.Language = language;  
        } 



        [AllowAnonymous]
        public async Task<IActionResult> UserLogin()
        {
            await ConfigLanguage();

            if (_options.Value.AllowAnonymous) {
                HttpContext.SetCookie (BasicConfig.LoginCookieId, "admin", 60 * 30 * 7);
                return Redirect ("/HttpReports");
            }
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