using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryConfigurationProvider : ConfigurationProvider
    {
        internal JsonRepositoryConfigurationSource Source { get; }

        public JsonRepositoryConfigurationProvider(JsonRepositoryConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override void Load()
        {
            Load(reload: false);
        }

        internal void Load(bool reload)
        {
            var jsonString = GetJsonString();

            if (string.IsNullOrWhiteSpace(jsonString))
            {
                if (Source.Optional || reload) // Always optional on reload
                {
                    Data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var error = new StringBuilder(string.Format(Errors.@FailedToGetJsonString, Source.Key));
                    if (Source.JsonConfigurationRepository == null)
                    {
                        error.Append(Errors.@JsonConfigurationRepositoryIsNull);
                    }

                    HandleException(ExceptionDispatchInfo.Capture(new JsonRepositoryLoadException(error.ToString())));
                }
            }
            else
            {
                try
                {
                    Data = JsonRepositoryConfigurationJsonStringParser.Parse(jsonString);
                }
                catch (JsonException e)
                {
                    throw new FormatException(Errors.ErrorJsonParseError, e);
                }
            }
        }

        private string GetJsonString()
        {
            string jsonString;
            try
            {
                jsonString = Source.JsonConfigurationRepository.Get(Source.Key);
            }
            catch (Exception e)
            {
                jsonString = string.Empty;
            }
            return jsonString;
        }

        private void HandleException(ExceptionDispatchInfo info)
        {
            bool ignoreException = false;
            if (Source.OnLoadException != null)
            {
                var exceptionContext = new JsonRepositoryLoadExceptionContext()
                {
                    Provider = this,
                    Exception = info.SourceException
                };
                Source.OnLoadException?.Invoke(exceptionContext);
                ignoreException = exceptionContext.Ignore;
            }

            if (!ignoreException)
            {
                info.Throw();
            }
        }
    }
}