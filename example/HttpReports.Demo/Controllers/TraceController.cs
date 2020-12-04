using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpReports.Demo.Controllers
{
    [Route("[controller]/[action]")]
    public class TraceController : Controller
    {  
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(new Random().Next(100,1000));

            HttpClient client = new HttpClient(); 

            var result = await client.GetStringAsync("http://localhost:5010/Trace/Get1");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get1()
        {
            await Task.Delay(new Random().Next(100, 1000));

            HttpClient client = new HttpClient();

            var result = await client.GetStringAsync("http://localhost:5010/Trace/Get2");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get2()
        {
            await Task.Delay(new Random().Next(100, 1000));

            HttpClient client = new HttpClient();

            var result = await client.GetStringAsync("http://localhost:5010/Trace/Get3");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get3()
        {
            await Task.Delay(new Random().Next(100, 1000));

            return Ok(new { code = 1,msg = "ok" });

        }  
    }
}
