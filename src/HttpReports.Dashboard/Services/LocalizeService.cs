using HttpReports.Core.Config;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpReports.Dashboard.Services
{
    public class LocalizeService
    { 
        private readonly Dictionary<string, Localize> _localize = new Dictionary<string, Localize>();
        private readonly IHttpReportsStorage _storage;
        private readonly ILogger<LocalizeService> _logger;

        public Localize Current { get; private set; }

        public IEnumerable<string> Langs => _localize.Keys;

        public LocalizeService(IHttpReportsStorage storage, ILogger<LocalizeService> logger)
        { 
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger;
        }

        public async Task InitAsync()
        {
            var assembly = GetType().Assembly;
            var files = assembly.GetManifestResourceNames().Where(m => m.StartsWith("HttpReports.Dashboard.HttpReportsStaticFiles.Content.Lang.") && m.EndsWith(".json"));
            foreach (var item in files)
            {
                var name = item.Replace("HttpReports.Dashboard.HttpReportsStaticFiles.Content.Lang.", string.Empty).Replace(".json", string.Empty);
                using var memory = new MemoryStream();
                using var stream = assembly.GetManifestResourceStream(item);
                await stream.CopyToAsync(memory);
                LoadLocalize(name, Encoding.UTF8.GetString(memory.ToArray()));
            }

            Current = _localize.First().Value;

            var lang = await _storage.GetSysConfig(BasicConfig.Language);  

            await SetLanguageAsync(lang);
        }

        public bool TryGetLanguage(string language, out Localize localize)
        {
            language = language.ToLowerInvariant();
            return _localize.TryGetValue(language, out localize);
        }

        public async Task<Localize> SetLanguageAsync(string language)
        {
            language = language.ToLowerInvariant();
            if (!Langs.Any(m => string.Equals(m, language)))
            {
                var localize = _localize.First();
                Current = localize.Value;
                await _storage.SetLanguage(localize.Key);
            }
            else
            {
                await _storage.SetLanguage(language);
                Current = _localize[language];
            }

            return Current;
        }

        private void LoadLocalize(string name, string json)
        {
            name = name.ToLowerInvariant();
            var resource = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            if (_localize.ContainsKey(name))
            {
                _localize[name] = new Localize(resource);
            }
            else
            {
                _localize.Add(name, new Localize(resource));
            }
        } 
      
    }
}
