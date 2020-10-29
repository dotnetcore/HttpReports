using System;
using System.Collections.Generic;
using System.Text;

namespace HttpReports.Core.ViewModels
{
    public enum HealthStatusEnum
    {
        None = 0,
        IsPassing = 1,
        IsWarning = 2,
        IsCritical = 3
    }
}
