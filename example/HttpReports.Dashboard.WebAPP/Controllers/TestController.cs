using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.WebAPP.Controllers
{ 
    public class TestController : Controller
    {
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

            return File(fileStream,"application/octet-stream"); 
        }


        [HttpGet("ErrorTest")]
        public async Task<IActionResult> Error()
        { 
            throw new Exception("error");

            return await Task.FromResult(Content("ok"));
             
        }


        [HttpGet("Test1")]
        public async Task<IActionResult> Test1()
        { 
            return await Task.FromResult(Content("Test1"));

        }

        [HttpPost("Test2")]
        public async Task<IActionResult> Test2()
        {
            return await Task.FromResult(Json(new { code = 1,msg = "ok"  }));

        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(User user)
        {
            return await Task.FromResult(Json(new { code = 1, msg = "ok" }));

        }
    }

    public class User
    {
        public string Name { get; set; }

        public int Age { get; set; }

    }

}
