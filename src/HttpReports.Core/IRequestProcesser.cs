using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace HttpReports
{
 
    public interface IRequestProcesser
    { 
        void Process(HttpContext context);  


    }
}