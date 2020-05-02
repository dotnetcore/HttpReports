using HttpReports.Core.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services.Language
{
    public class LanguageService
    {
        private readonly IHttpReportsStorage _storage;
        private readonly ChineseLanguage _chineseLanguage;
        private readonly EnglishLanguage _englishLanguage;  
        public LanguageService(IHttpReportsStorage storage, ChineseLanguage chineseLanguage, EnglishLanguage englishLanguage)
        {
            _storage = storage;
            _chineseLanguage = chineseLanguage;
            _englishLanguage = englishLanguage;
        }

        public async Task<ILanguage> GetLanguage()
        {
            var language = await _storage.GetSysConfig(BasicConfig.Language);

            if (language == "English")
            {
                return _englishLanguage as ILanguage;
            }

            if (language == "Chinese")
            {
                return _chineseLanguage as ILanguage;
            } 

            return null; 
        }  
    }
}
