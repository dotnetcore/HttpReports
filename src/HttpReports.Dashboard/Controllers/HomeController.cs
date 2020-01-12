using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpReportsStorage _storage;

        public HomeController(IHttpReportsStorage storage)
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

        public async Task<IActionResult> AddMonitor(int Id = 0)
        {
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            ViewBag.Id = Id;

            return View();
        }

        public async Task<IActionResult> Detail()
        {
            var nodes = (await _storage.GetNodesAsync()).Select(m => m.Name).ToList();

            ViewBag.nodes = nodes;

            return View();
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}