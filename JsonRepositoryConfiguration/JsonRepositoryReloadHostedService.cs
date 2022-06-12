using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryReloadHostedService : BackgroundService
    {
        private IEnumerable<JsonRepositoryConfigurationProvider> Providers { get; }

        public JsonRepositoryReloadHostedService(IEnumerable<JsonRepositoryConfigurationProvider> providers) =>
            Providers = providers ?? throw new ArgumentNullException(nameof(providers));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
            await Task.WhenAll(Providers.Select(p => p.ReloadOnChangeAsync(stoppingToken)).ToArray());
    }
}