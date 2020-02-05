﻿using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    public class HttpReportsHomeController : Controller
    {
        private readonly IHttpReportsStorage _storage;

        public HttpReportsHomeController(IHttpReportsStorage storage)
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

        public async Task<IActionResult> EditMonitor(int Id = 0)
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


        public IActionResult Test()
        {
            return View();
        }
    }
}