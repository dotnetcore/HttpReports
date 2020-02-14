using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Interface
{
    public interface ISysUser
    { 
        string Id { get; set; }

        string UserName { get; set; }

        string Password { get; set; }

    }
}
