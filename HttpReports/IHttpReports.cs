using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports
{
    public interface IHttpReports
    {

        void Invoke(HttpContext context,double Milliseconds,IConfiguration configuration);


        void Init(IConfiguration config);

    }
}
