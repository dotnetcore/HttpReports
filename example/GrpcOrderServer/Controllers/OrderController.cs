using Grpc.Net.Client;
using GrpcUserServer.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GrpcOrderServer.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> GetOrderInfo()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5002");

            var client = new UserService.UserServiceClient(channel);

            var user = await client.GetUserInfoAsync(new GetUserInfoRequest { Id = 10 });

            return Ok(user);
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> GetOrderInfo2()
        {
            var client = new HttpClient();

            var result = await client.GetStringAsync("https://localhost:5002/api/User/GetUserInfo");  

            return Ok(result);
        }

    }
}
