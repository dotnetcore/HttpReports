using HttpReports.Core;
using HttpReports.Core.Models;
using HttpReports.Storage.PostgreSQL;
using Microsoft.Extensions.DependencyInjection;
using Snowflake.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Mock.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await APITest(); 
            System.Console.ReadKey();


            var a = 199;
            var b = 10000;
            var c = 60000; 

            if (args.ToList().Any())
            {
                a = Convert.ToInt32(args[0]);
                b = Convert.ToInt32(args[1]);
                c = Convert.ToInt32(args[2]); 
            } 

            var services = new ServiceCollection();
            services.AddOptions();
            services.AddLogging();

            services.Configure<PostgreStorageOptions>(o =>
            {
                o.ConnectionString = "Host=localhost;Port=5432;Username=postgres;Password=123456;Database=HttpReports;";
                o.DeferSecond = 5;
                o.DeferThreshold = 5; 

            });

            services.AddTransient<PostgreSQLStorage>();

            var _storage = services.BuildServiceProvider().GetRequiredService<PostgreSQLStorage>();

            _ = Task.Run(async () => {

                while (true)
                {     
                    var res = await _storage.UpdateLoginUser(new SysUser
                    {
                        Id = 1336647892979257344,
                        UserName = "admin",
                        Password = MD5_32("123456")

                   });

                    System.Console.WriteLine(res);

                    await Task.Delay(3000);

                }  
            
            });


            await InsertTestAsync(_storage,a,b,c);

            System.Console.ReadKey();
          
        }

        static async Task APITest()
        {
            for (int i = 0; i < 50; i++)
            {
                _ = Task.Run(async () => {

                    HttpClient client = new HttpClient();

                    while (true)
                    {
                        await Task.Delay(new Random().Next(10,100));

                        HttpContent content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new { TemperatureC = 99, Summary = "Summary" }), System.Text.Encoding.UTF8, "application/json");

                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                        content.Headers.Add(BasicConfig.TransportType, typeof(Performance).Name);

                        try
                        {
                            await client.PostAsync("http://localhost:5010/WeatherForecast", content);
                        }
                        catch (Exception ex)
                        {

                             
                        }  
                    } 
                
                }); 

            }   

        }
       


        public static Task InsertTestAsync(PostgreSQLStorage storage,int a,int b,int c)
        { 
            IdWorker idWorker = new IdWorker(new Random().Next(1,30), new Random().Next(0,30)); 

            var startTime = DateTime.Now.AddSeconds(-1);
            var count = 100000;
            var random = new Random();

            string[] Services = { "User", "User", "User", "User", "User", "Order", "Order", "Order", "Order", "Weixin", "Weixin", "Payment", "Log", "Log", "Log", "DataCenter", "Student", "Student", "Master", "Product", "Sale", "Sass" };
            string[] LoginUsers = { "Jack", "Blues", "Mark", "Tom", "Cat" };
            string[] ParentServices = { "User", "Order", "Weixin", "Payment", "Log", "DataCenter" };
            string[] LocalIPs = { "192.168.1.1", "192.168.1.1", "192.168.1.1", "192.168.1.1", "192.168.1.1", "192.168.1.1", "192.168.1.2", "192.168.1.2", "192.168.1.2", "192.168.1.3", "192.168.1.3", "192.168.1.4", "192.168.1.5", "192.168.1.6", "192.168.1.7", "192.168.1.8", "192.168.1.9", "192.168.1.10" };

            string[] Route = { "/User/Login", "/User/Payment", "/User/Payment", "/User/Payment", "/User/Payment", "/User/Payment", "/User/Search", "/User/Search", "/User/Search", "/Data/QueryData", "/Data/GetCofnig", "/Data/GetCofnig", "/User/LoadData", "/User/LoadData1", "/User/LoadData2", "/User/LoadData3", "/User/LoadData4" };

            int[] LocalPort = { 8801, 8801, 8801, 8801, 8802, 8802, 8802, 8803, 8803, 8804, 8805, 8806, 8007, 8008, 8009 }; 

            //Task.Run(() => Insert());
            //Task.Run(() => Insert());
            //Task.Run(() => Insert());
            //Task.Run(() => Insert());
            //Task.Run(() => Insert());
            //Task.Run(() => Insert());

            Insert();

            return Task.CompletedTask;

            void Insert()
            {
                for (int i = 0; i < 10000000; i++)
                {
                    List<Core.RequestBag> requestBags = new List<Core.RequestBag>();

                    for (int c = 0; c < a; c++)
                    {
                        var _Service = Services[new Random().Next(0, Services.Length - 1)];
                        var _ParentService = ParentServices[new Random().Next(0, ParentServices.Length - 1)];
                        var _url = Route[new Random().Next(0, Route.Length - 1)];

                        if (_ParentService == _Service) _ParentService = string.Empty;

                        var info = new RequestInfo
                        {
                            Id = idWorker.NextId(),
                            ParentId = 0,
                            Service = _Service,
                            ParentService = _ParentService,
                            Route = _url,
                            Url = _url,
                            RequestType = "http",
                            Method = "POST",
                            LoginUser = LoginUsers[new Random().Next(0, LoginUsers.Length - 1)], 
                            Milliseconds = _Service switch
                            { 
                                "User" => new Random().Next(1400, 1600),
                                "Order" => new Random().Next(1200, 1600),
                                "Weixin" => new Random().Next(600, 1600),
                                "Log" => new Random().Next(100, 500),
                                "Payment" => new Random().Next(100, 800),
                                _ => new Random().Next(1, 1600)

                            },
                            StatusCode = _Service switch
                            {
                                "User" => new Random().Next(1, 10) > 1 ? 200 : 500,
                                "Order" => new Random().Next(1, 10) > 3 ? 200 : 500,
                                "Weixin" => new Random().Next(1, 10) > 7 ? 200 : 500,
                                "Log" => new Random().Next(1, 10) > 6 ? 200 : 500,
                                "Payment" => new Random().Next(1, 10) > 4 ? 200 : 500,
                                _ => new Random().Next(1, 10) > 5 ? 200 : 500

                            },
                            RemoteIP = "192.168.1.1",
                            Instance = LocalIPs[new Random().Next(0, LocalIPs.Length - 1)] + ":" + LocalPort[new Random().Next(0, LocalPort.Length - 1)],
                            CreateTime = DateTime.Now

                        };

                        requestBags.Add(new Core.RequestBag(info, null));

                    } 

                    storage.AddRequestInfoAsync(requestBags, System.Threading.CancellationToken.None).Wait();

                    System.Console.WriteLine(i * a);

                    Task.Delay(new Random().Next(b, c)).Wait();

                }

            }

        }


        public static string MD5_16(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string val = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(source)), 4, 8).Replace("-", "").ToLower();
            return val;
        }

        public static string MD5_32(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(source);
            string result = BitConverter.ToString(md5.ComputeHash(bytes));
            return result.Replace("-", "").ToLower();
        }


    }
}
