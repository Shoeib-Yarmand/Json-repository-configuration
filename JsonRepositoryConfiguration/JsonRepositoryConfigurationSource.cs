using System;
using Microsoft.Extensions.Configuration;

namespace JsonRepositoryConfiguration
{
    /// <summary>
    /// Represents a JSON Repository as an <see cref="IConfigurationSource"/>.
    /// </summary>
    public class JsonRepositoryConfigurationSource : IConfigurationSource
    {
        /// <summary>
        /// An instance to get JSON String from.
        /// </summary>
        public IJsonConfigurationRepository JsonConfigurationRepository { get; set; }

        /// <summary>
        /// The key to pass to <see cref="IJsonConfigurationRepository"/> to get JSON string back.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Determines if loading the config for the key is optional.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Determines whether the source will be reloaded if the result from <see cref="IJsonConfigurationRepository"/> for the key changes.
        /// <see cref="IServiceCollection"/>.<see cref="AddJsonRepositoryReloadOnChange"/> MUST be called to register needed services in order for this to work.
        /// </summary>
        public bool ReloadOnChange { get; set; }

        /// <summary>
        /// Number of seconds between checking <see cref="IJsonConfigurationRepository"/> for changes. Default is 60.
        /// </summary>
        public int ChangeCheckInterval { get; set; } = 60;

        /// <summary>
        /// Will be called if an uncaught exception occurs in <see cref="JsonRepositoryConfigurationProvider.Load()"/>.
        /// </summary>
        public Action<JsonRepositoryLoadExceptionContext> OnLoadException { get; set; }

        /// <summary>
        /// Builds the <see cref="IConfigurationProvider"/> for this source.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        /// <returns>A <see cref="IConfigurationProvider"/></returns>
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            EnsureDefaults(builder);
            return new JsonRepositoryConfigurationProvider(this);
        }

        /// <summary>
        /// Called to use any default settings on the builder like the <see cref="IJsonConfigurationRepository"/> or <see cref="OnLoadException"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/>.</param>
        public void EnsureDefaults(IConfigurationBuilder builder)
        {
            JsonConfigurationRepository = JsonConfigurationRepository ?? builder.GetJsonConfigurationRepository();
            OnLoadException = OnLoadException ?? builder.GetJsonRepositoryLoadExceptionHandler();
        }

    }
}
