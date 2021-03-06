﻿using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BingeBuddyNg.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace BingeBuddyNg.Infrastructure
{
    public class TranslationService : ITranslationService
    {
        private ConcurrentDictionary<string, Task<JObject>> translation = new ConcurrentDictionary<string, Task<JObject>>();

        private readonly IStorageAccessService storageAccessService;
        private readonly ILogger<TranslationService> logger;

        public TranslationService(IStorageAccessService storageAccessService, ILogger<TranslationService> logger)
        {
            this.storageAccessService = storageAccessService ?? throw new ArgumentNullException(nameof(storageAccessService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> GetTranslationAsync(string language, string key, params object[] values)
        {
            language = language ?? Shared.Constants.DefaultLanguage;
            var translationTask = this.translation.GetOrAdd(language, GetTranslationFile(language));

            var translationObject = await translationTask;

            var val = translationObject.SelectToken(key);

            var translationString = val.ToString();
            if (values != null && values.Length > 0)
            {
                translationString = string.Format(translationString, values);
            }
            logger.LogDebug($"Successfully retrieved value [{translationString}] for key [{key}] in language [{language}].");
            return translationString;
        }

        private async Task<JObject> GetTranslationFile(string language)
        {
            var fileContent = await this.storageAccessService.GetFileFromStorageAsync("$web", $"assets/i18n/{language}.json");
            JObject jObject = JObject.Parse(fileContent);
            return jObject;
        }
    }
}