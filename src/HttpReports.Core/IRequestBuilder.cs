using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HttpReports.Core;
using HttpReports.Core.Models;
using Microsoft.AspNetCore.Http;

namespace HttpReports
{
    
    public interface IRequestBuilder
    { 
        (RequestInfo,RequestDetail) Build(HttpContext context); 
    }
}