using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.Interface
{
    public interface ISysConfig
    {
        string Id { get; set; }
        string Key { get; set; }

        string Value { get; set; }
    }
}
