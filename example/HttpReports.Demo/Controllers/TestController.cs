using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Demo.Controllers
{
    public class TestController : Controller
    {
        private readonly IWebHostEnvironment hostingEnv;

        public TestController(IWebHostEnvironment webHostEnvironment)
        {
            hostingEnv = webHostEnvironment;
        }


        [HttpPost("UploadFilesAsync")]
        public async Task<IActionResult> Uploads(IFormFileCollection fileCollection)
        {
            var files = HttpContext.Request.Form.Files;

            var dictionary = new Dictionary<string, Stream>();
            foreach (var file in files)
            {
                dictionary.Add(file.FileName, file.OpenReadStream());
            }

            foreach (var stream in dictionary.Values)
            {
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
            }

            return Json(new { msg = "OK" });
        }


        [HttpGet("Download")]
        public async Task<IActionResult> Download()
        {
            var filePath = @"E:\SpringLee\HttpReports\example\HttpReports.Dashboard.WebAPP\appsettings.json";

            FileStream fileStream = new FileStream(filePath, FileMode.Open);

            return File(fileStream, "application/octet-stream");
        }


        [HttpGet("EPPLus")]
        public async Task<IActionResult> EPPLus()
        {
            string sWebRootFolder = hostingEnv.ContentRootPath;
            string sFileName = $@"qmhuangtext{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            var path = Path.Combine(sWebRootFolder, sFileName);
            FileInfo file = new FileInfo(path);
            //构建数据
            List<Person> list1 = new List<Person>()
               {
                    new Person { Name = "123", Sex = "男" },
                    new Person { Name = "234", Sex = "男" },
                    new Person { Name = "345", Sex = "女" }
               };
            if (file.Exists)
            {
                file.Delete();
                file = new FileInfo(path);
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                //创建sheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetname");
                worksheet.Cells.LoadFromCollection(list1, true);
                package.Save(); //Save the workbook.
            }
            return File(new FileStream(Path.Combine(sWebRootFolder, sFileName), FileMode.Open), "application/octet-stream", $"excel导出测试{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx");
        }




        [HttpGet("ErrorTest")]
        public async Task<IActionResult> Error()
        {
            throw new Exception("error");
        }


        [HttpGet("Test1")]
        public async Task<IActionResult> Test1()
        {
            return await Task.FromResult(Content("Test1"));

        }

        [HttpGet("Test2")]
        public async Task<IActionResult> Test2()
        {
            return await Task.FromResult(Json(new { code = 1, msg = "ok", name = "李先生" }));
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(User user)
        {
            return await Task.FromResult(Json(new { code = 1, msg = "ok" }));
        }


        [HttpGet("Query")]
        public async Task<IActionResult> Query()
        {
            return await Task.FromResult(Json(new { code = 1, msg = "ok" }));
        }
    }

    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

    }

    public class Person
    {
        public string Name { get; set; }

        public string Sex { get; set; }

    }
}
