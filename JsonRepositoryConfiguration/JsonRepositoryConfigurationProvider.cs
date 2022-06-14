using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryConfigurationProvider : ConfigurationProvider, IDisposable
    {
        internal JsonRepositoryConfigurationSource Source { get; }
        private IDisposable _changeTokenRegistration;
        private CancellationTokenSource _cts;

        public JsonRepositoryConfigurationProvider(JsonRepositoryConfigurationSource source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
        }

        public override void Load()
        {
            Load(reload: false);
        }

        private void Load(bool reload)
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

            OnReload();
        }

        internal async Task ReloadOnChangeAsync(CancellationToken stoppingToken)
        {
            if (!Source.ReloadOnChange)
                return;

            _changeTokenRegistration = ChangeToken.OnChange(CreateChangeToken, () => Load(reload: true));

            var oldHash = ToSha256(GetJsonString());
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Source.ChangeCheckInterval * 1000, stoppingToken);
                var newHash = ToSha256(GetJsonString());
                if (oldHash != newHash)
                    _cts?.Cancel();
                oldHash = newHash;
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

        private IChangeToken CreateChangeToken()
        {
            _cts = new CancellationTokenSource();
            return new CancellationChangeToken(_cts.Token);
        }

        private string ToSha256(string input)
        {
            using var sha256 = SHA256.Create();
            var builder = new StringBuilder();
            foreach (var b in sha256.ComputeHash(Encoding.UTF8.GetBytes(input)))
                builder.Append(b.ToString("X2"));
            return builder.ToString();
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

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            _changeTokenRegistration?.Dispose();
        }
    }
}