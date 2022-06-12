using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryReloadHostedService : BackgroundService
    {
        private JsonRepositoryConfigurationProvider Provider { get; }
        private IDisposable _changeTokenRegistration;
        private CancellationTokenSource _cts;

        public JsonRepositoryReloadHostedService(JsonRepositoryConfigurationProvider provider)
        {
            Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _changeTokenRegistration = ChangeToken.OnChange(CreateChangeToken, () => Provider.Load(reload: true));

            if (!Provider.Source.ReloadOnChange) 
                return;

            var oldHash = ToSha256(GetJsonString());
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(Provider.Source.ChangeCheckInterval * 1000, stoppingToken);
                var newHash = ToSha256(GetJsonString());
                if (oldHash != newHash)
                    _cts?.Cancel();
                oldHash = newHash;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _changeTokenRegistration?.Dispose();
            await base.StopAsync(cancellationToken);
        }

        private string GetJsonString()
        {
            string jsonString;
            try
            {
                jsonString = Provider.Source.JsonConfigurationRepository.Get(Provider.Source.Key);
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
    }
}