using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HttpReports.Dashboard.Models;
using HttpReports.Dashboard.Services;
using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private DataService _dataService;

        public HomeController(DataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public IActionResult Trend()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public IActionResult Monitor()
        {
            var jobs = _dataService.GetJobs();

            List<JobRequest> list = new List<JobRequest>();

            foreach (var item in jobs)
            {
                list.Add(new JobRequest
                {
                    Id = item.Id,
                    Title = item.Title,
                    CronLike = _dataService.ParseJobCronString(item.CronLike),
                    Email = item.Emails,
                    Mobiles = item.Mobiles,
                    Status = item.Status,
                    Node = item.Servers

                });
            }

            ViewBag.list = list;

            return View();
        }

        public IActionResult AddMonitor(int Id = 0)
        { 
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            ViewBag.Id = Id;

            return View();
        }


        public IActionResult Detail()
        {
            var nodes = _dataService.GetNodes();

            ViewBag.nodes = nodes;

            return View();
        }

        public IActionResult Test()
        {
            return View();
        }

    }
}