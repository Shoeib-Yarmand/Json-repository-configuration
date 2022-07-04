namespace JsonRepositoryConfiguration
{
    public interface IJsonConfigurationRepository
    {
        /// <summary>
        /// Gets a JSON text by its key. It can fetch the JSON data from any internal or external sources, including a Database call or an API call.
        /// This is used for both loading the data for the first time, and refreshing data in scheduled intervals if ReloadOnChange is activated.
        /// </summary>
        /// <param name="key">The key for the JSON config</param>
        /// <returns>Entire JSON as string (serialized/stringify)</returns>
        public string GetByKey(string key);
    }
}

