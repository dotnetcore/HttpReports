using HttpReports.Dashboard.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Abstractions
{
    public interface ILocalizeService
    { 
        Localize Current { get; set; }

        Task InitAsync();

        bool TryGetLanguage(string language, out Localize localize);

        Task<Localize> SetLanguageAsync(string language);

        void LoadLocalize(string name, string json);
    }
}
