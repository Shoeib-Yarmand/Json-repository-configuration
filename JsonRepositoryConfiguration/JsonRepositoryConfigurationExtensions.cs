﻿using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JsonRepositoryConfiguration
{
    public static class JsonRepositoryConfigurationExtensions
    {
        private static string JsonConfigurationRepositoryKey = "JsonConfigurationRepository";
        private static string JsonRepositoryLoadExceptionHandlerKey = "JsonRepositoryLoadExceptionHandler";

        public static IConfigurationBuilder AddJsonRepository(this IConfigurationBuilder builder, string key,
            IJsonConfigurationRepository repository, bool optional, bool reloadOnChange, int changeCheckInterval)
        {
            return builder.AddJsonRepository(s =>
            {
                s.Key = key;
                s.JsonConfigurationRepository = repository;
                s.Optional = optional;
                s.ReloadOnChange = reloadOnChange;
                s.ChangeCheckInterval = changeCheckInterval;
            });
        }

        public static IConfigurationBuilder AddJsonRepository(this IConfigurationBuilder builder,
            Action<JsonRepositoryConfigurationSource> configureSource)
            => builder.Add(configureSource);

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

        public static Action<JsonRepositoryLoadExceptionContext> GetJsonRepositoryLoadExceptionHandler(
            this IConfigurationBuilder builder)
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

        public static IServiceCollection AddJsonRepositoryReloadOnChange(this IServiceCollection services, IConfiguration configuration)
        {
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