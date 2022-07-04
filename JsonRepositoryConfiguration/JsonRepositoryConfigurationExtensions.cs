using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JsonRepositoryConfiguration
{
    /// <summary>
    /// Extension methods for adding <see cref="JsonRepositoryConfigurationProvider"/> and registering needed services in order to enable Reload On Change feature.
    /// </summary>
    public static class JsonRepositoryConfigurationExtensions
    {
        private static string JsonConfigurationRepositoryKey = "JsonConfigurationRepository";
        private static string JsonRepositoryLoadExceptionHandlerKey = "JsonRepositoryLoadExceptionHandler";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="key">The key to pass to <paramref name="repository"/> to access the JSON.</param>
        /// <param name="repository">The <see cref="IJsonConfigurationRepository"/> to use to access the JSON.</param>
        /// <param name="optional">Whether this configuration is optional.</param>
        /// <param name="reloadOnChange">Whether the configuration should be reloaded periodically.
        /// <see cref="IServiceCollection"/>.<see cref="AddJsonRepositoryReloadOnChange"/> MUST be called to register needed services in order for this to work.</param>
        /// <param name="changeCheckInterval">The interval (in seconds) between each call to <paramref name="repository"/> to check for any changes and reload the configuration if it has changed.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonRepository(this IConfigurationBuilder builder, string key,
            IJsonConfigurationRepository repository, bool optional = true, bool reloadOnChange = true, int changeCheckInterval = 60)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(Errors.JsonRepositoryKeyCannotBeNullOrEmpty, nameof(key));
            }

            if (repository == null)
            {
                throw new ArgumentException(Errors.JsonConfigurationRepositoryIsNull, nameof(repository));
            }

            return builder.AddJsonRepository(s =>
            {
                s.Key = key;
                s.JsonConfigurationRepository = repository;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ChangeCheckInterval = changeCheckInterval;
            });
        }

        /// <summary>
        /// Adds a JSON Repository Configuration Source to <paramref name="builder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to add to.</param>
        /// <param name="configureSource">Configures the source.</param>
        /// <returns>The <see cref="IConfigurationBuilder"/>.</returns>
        public static IConfigurationBuilder AddJsonRepository(this IConfigurationBuilder builder,
            Action<JsonRepositoryConfigurationSource> configureSource)
            => builder.Add(configureSource);

        /// <summary>
        /// Gets the default registered <see cref="IJsonConfigurationRepository"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IJsonConfigurationRepository GetJsonConfigurationRepository(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.Properties.TryGetValue(JsonConfigurationRepositoryKey, out object provider))
            {
                return provider as IJsonConfigurationRepository;
            }

            return null;
        }

        /// <summary>
        /// Gets the default registered handler for <see cref="JsonRepositoryLoadExceptionContext"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IConfigurationBuilder"/> to search for the handler.</param>
        /// <returns>The handler for <see cref="JsonRepositoryLoadExceptionContext"/></returns>
        public static Action<JsonRepositoryLoadExceptionContext> GetJsonRepositoryLoadExceptionHandler(this IConfigurationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (builder.Properties.TryGetValue(JsonRepositoryLoadExceptionHandlerKey, out object handler))
            {
                return handler as Action<JsonRepositoryLoadExceptionContext>;
            }

            return null;
        }

        /// <summary>
        /// Registers needed services in order to enable Reload On Change feature. It needs to be called only once no matter how many <see cref="JsonRepositoryConfigurationSource"/> has been registered.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to register the service.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> to search for all registered <see cref="JsonRepositoryConfigurationSource"/></param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddJsonRepositoryReloadOnChange(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var configurationRoot = (IConfigurationRoot)configuration;

            var roloadableConfigurationProviders = configurationRoot.Providers
                .Where(cp => cp is JsonRepositoryConfigurationProvider)
                .Cast<JsonRepositoryConfigurationProvider>()
                .Where(cp => cp.Source.ReloadOnChange);

            services.AddHostedService(p => new JsonRepositoryReloadHostedService(roloadableConfigurationProviders));

            return services;
        }
    }
}