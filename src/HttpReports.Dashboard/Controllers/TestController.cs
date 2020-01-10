using HttpReports.Dashboard.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    [AllowAnonymous]
    public class TestController : Controller
    {
        private DataService _dataService;

        public TestController(DataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult A1()
        {
            System.Threading.Thread.Sleep(1000);

            return Json(new { code = 1 });
        }

        public IActionResult A2()
        {
            System.Threading.Thread.Sleep(2000);

            return Json(new { code = 1 });
        }

        public IActionResult A3()
        {
            System.Threading.Thread.Sleep(3000);

            return Json(new { code = 1 });
        }

        public IActionResult A4()
        {
            System.Threading.Thread.Sleep(4000);

            return Json(new { code = 1 });
        }

        public IActionResult A5()
        {
            System.Threading.Thread.Sleep(5000);

            return Json(new { code = 1 });
        }

        public IActionResult A6()
        {
            System.Threading.Thread.Sleep(6000);

            return Json(new { code = 1 });
        }

        public IActionResult A7()
        {
            System.Threading.Thread.Sleep(7000);

            return Json(new { code = 1 });
        }

        public IActionResult A8()
        {
            System.Threading.Thread.Sleep(8000);

            return Json(new { code = 1 });
        }

        public IActionResult A9()
        {
            System.Threading.Thread.Sleep(9000);

            return Json(new { code = 1 });
        }

        public IActionResult A10()
        {
            System.Threading.Thread.Sleep(10000);

            return Json(new { code = 1 });
        }
    }
}