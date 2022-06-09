using System;

namespace JsonRepositoryConfiguration
{
    public class JsonRepositoryLoadException : Exception
    {
        public JsonRepositoryLoadException(string message) : base(message)
        {
        }
    }
}