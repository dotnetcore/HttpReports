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
        //private readonly string url = "http://122.51.188.23:8080";
        private readonly string url = "http://localhost:5010";

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(new Random().Next(100,1000));

            HttpClient client = new HttpClient(); 

            var result = await client.GetStringAsync($"{url}/Trace/Get1");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get1()
        {
            var c = HttpContext;

            await Task.Delay(new Random().Next(100, 1000));

            HttpClient client = new HttpClient();

            _ = Task.Run(async ()=> {

                var result = await client.GetStringAsync($"{url}/Trace/Get4"); 

            }); 
            

            var result = await client.GetStringAsync($"{url}/Trace/Get2");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get2()
        {
            await Task.Delay(new Random().Next(100, 1000));

            HttpClient client = new HttpClient();

            var result = await client.GetStringAsync($"{url}/Trace/Get3");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get3()
        {
            await Task.Delay(new Random().Next(100, 1000));

            return Ok(new { code = 1,msg = "ok" });

        }


        [HttpGet]
        public async Task<IActionResult> Get4()
        {
            await Task.Delay(new Random().Next(5000, 8000));

            return Ok(new { code = 1, msg = "ok" });

        }
    }
}
