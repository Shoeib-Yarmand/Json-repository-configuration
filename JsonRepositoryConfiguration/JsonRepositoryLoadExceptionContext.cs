using System;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryLoadExceptionContext
    {
        public JsonRepositoryConfigurationProvider Provider { get; set; }

        /// <summary>
        /// The exception that occurred in Load.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// If true, the exception will not be rethrown.
        /// </summary>
        public bool Ignore { get; set; }
    }
}