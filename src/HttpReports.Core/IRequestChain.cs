using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core
{
    public interface IRequestChain
    { 
        string Id { get; set; }

        string ParentId { get; set; } 

        string Service { get; set; }

        string Instance { get; set; } 

        string Component { get; set; }  

        string Error { get; set; }  
        
        string ErrorMessage { get; set; }

        int IsSuccess { get; set; }

        string Value { get; set; } 

        int Milliseconds { get; set; }

        DateTime CreateTime { get; set; }

    }
}
