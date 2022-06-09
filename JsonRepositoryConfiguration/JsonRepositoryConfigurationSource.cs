using System;
using Microsoft.Extensions.Configuration;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// An instance to get JsonString from.
        /// </summary>
        public IJsonConfigurationRepository JsonConfigurationRepository { get; set; }

        /// <summary>
        /// The key to pass to JsonConfigurationRepository to get Json string back.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Determines if loading the config for the key is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Determines whether the source will be loaded if the result from IJsonStringProvider for the key changes.
        /// </summary>
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// Number of seconds between checking IJsonStringProvider for changes. Default is 60.
        /// </summary>
        public int ChangeCheckInterval { get; set; } = 60;

        /// <summary>
        /// Will be called if an uncaught exception occurs in FileConfigurationProvider.Load.
        /// </summary>
        public Action<JsonRepositoryLoadExceptionContext> OnLoadException { get; set; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new JsonRepositoryConfigurationProvider(this);
        }

        public void EnsureDefaults(IConfigurationBuilder builder)
        {
            JsonConfigurationRepository = JsonConfigurationRepository ?? builder.GetJsonConfigurationRepository();
            OnLoadException = OnLoadException ?? builder.GetJsonRepositoryLoadExceptionHandler();
        }

    }
}
