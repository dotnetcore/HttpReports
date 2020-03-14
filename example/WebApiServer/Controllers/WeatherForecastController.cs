using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [HttpPost]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet]
        [HttpPost]
        public IActionResult Test(Entity entity)
        { 
            return Ok(new
            { 
                user = entity.User,
                password = entity.Password,
                type = "Response"

            });
           
        }

        [HttpGet]
        [HttpPost]
        public IActionResult Test1(Entity entity)
        {
            int a = Convert.ToInt32("PPP");

            return Ok(new
            {
                user = entity.User,
                password = entity.Password,
                type = "Response"

            });

        }

        public IActionResult Notify(Entity entity)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(entity));
            return Ok(entity);

        } 

    }

    
    public class Entity
    {
        public string User { get; set; }

        public string Password { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }


    }

}
