using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace HttpReports.Dashboard.WebAPP.Controllers
{
    [Route("[controller]/[action]")]
    public class TraceController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Get2()
        {
            await Task.Delay(500);

            HttpClient client = new HttpClient();

            var result = await client.GetStringAsync("http://localhost:5010/Trace/Get3");

            return Content(result);

        }


        [HttpGet]
        public async Task<IActionResult> Get3()
        { 
            await Task.Delay(2300);

            HttpClient client = new HttpClient();

            var result = await client.GetStringAsync(" http://localhost:5501/WeatherForecast/Get1"); 

            return await Task.FromResult(Content("OK")); 
        }

    }
}
