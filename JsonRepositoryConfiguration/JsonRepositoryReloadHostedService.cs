using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace JsonRepositoryConfiguration
{
    /// <summary>
    /// A <see cref="BackgroundService"/> based implementation to enable Reload On Change feature.
    /// </summary>
    public class JsonRepositoryReloadHostedService : BackgroundService
    {
        private IEnumerable<JsonRepositoryConfigurationProvider> Providers { get; }

        /// <summary>
        /// Creates and instance using <param name="providers"></param>
        /// </summary>
        /// <param name="providers">List of all <see cref="JsonRepositoryConfigurationProvider"/> which have <see cref="ReloadOnChange"/> set to true.</param>
        public JsonRepositoryReloadHostedService(IEnumerable<JsonRepositoryConfigurationProvider> providers) =>
            Providers = providers ?? throw new ArgumentNullException(nameof(providers));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
            await Task.WhenAll(Providers.Select(p => p.ReloadOnChangeAsync(stoppingToken)).ToArray());
    }
}