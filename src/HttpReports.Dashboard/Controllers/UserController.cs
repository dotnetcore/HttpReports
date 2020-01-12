using HttpReports.Dashboard.DTO;
using HttpReports.Dashboard.Implements;
using HttpReports.Dashboard.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.Controllers
{
    [AllowAnonymous]
    public class UserController : Controller
    {
        private HttpReportsConfig _config;

        public UserController(HttpReportsConfig config)
        {
            _config = config;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CheckLogin(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return Json(new HttpResult(-1, "账号或密码错误"));
            }

            if (_config.UserName == username && _config.Password == password)
            {
                HttpContext.SetCookie("login_info", username, 60 * 30 * 10);

                return Json(new HttpResult(1, "登录成功"));
            }

            return Json(new HttpResult(-1, "账号或密码错误"));
        }

        public IActionResult Logout()
        {
            HttpContext.DeleteCookie("login_info");

            return new RedirectResult("/User/Login");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Test()
        {
            return View();
        }
    }
}