using BingeBuddyNg.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace BingeBuddyNg.Services
{
    public class TranslationService : ITranslationService
    {
        public StorageAccessService StorageAccessService { get; }

        private ILogger<TranslationService> logger;
        private ConcurrentDictionary<string, Task<JObject>> translation = new ConcurrentDictionary<string, Task<JObject>>();

        public TranslationService(StorageAccessService storageAccessService, ILogger<TranslationService> logger)
        {
            this.StorageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetTranslationAsync(string language, string key, params object[] values)
        {
            language = language ?? Shared.Constants.DefaultLanguage;
            var translationTask = this.translation.GetOrAdd(language, GetTranslationFile(language));

            var translationObject = await translationTask;

            var translationString = translationObject[key].Value<string>();
            if(values != null && values.Length > 0)
            {
                translationString = string.Format(translationString, values);
            }
            logger.LogDebug($"Successfully retrieved value [{translationString}] for key [{key}] in language [{language}].");
            return translationString;
        }

        private async Task<JObject> GetTranslationFile(string language)
        {
            var fileContent = await this.StorageAccessService.GetFileFromStorageAsync("$web", $"assets/i18n/{language}.json");
            JObject jObject = JObject.Parse(fileContent);
            return jObject;
        }        
    }
}
