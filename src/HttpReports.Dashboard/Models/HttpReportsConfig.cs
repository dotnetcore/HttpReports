using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Models
{
    public class HttpReportsConfig
    { 
        public HttpReportsConfig(IConfiguration configuration)
        {
            this.UserName = configuration["HttpReportsConfig:UserName"];
            this.Password = configuration["HttpReportsConfig:Password"];   
        }  

       public string UserName { get; set; }

       public string Password { get; set; }   

    }  
}
