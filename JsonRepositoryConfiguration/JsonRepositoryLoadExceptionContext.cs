using System;

namespace JsonRepositoryConfiguration
{
    /// <summary>
    /// Contains information about a Repository load exception.
    /// </summary>
    public class JsonRepositoryLoadExceptionContext
    {
        /// <summary>
        /// The <see cref="JsonRepositoryConfigurationProvider"/> that caused the exception.
        /// </summary>
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