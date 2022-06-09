namespace JsonRepositoryConfiguration
{
    internal static class Errors
    {
        /// <summary>Top-level JSON element must be an object. Instead, '{0}' was found.</summary>
        internal static string ErrorInvalidTopLevelJsonElement => @"Top-level JSON element must be an object. Instead, '{0}' was found.";
        /// <summary>Could not parse the JSON file.</summary>
        internal static string ErrorJsonParseError => @"Could not parse the JSON string.";
        /// <summary>A duplicate key '{0}' was found.</summary>
        internal static string ErrorKeyIsDuplicated => @"A duplicate key '{0}' was found.";
        /// <summary>Unsupported JSON token '{0}' was found.</summary>
        internal static string ErrorUnsupportedJsonToken => @"Unsupported JSON token '{0}' was found.";

        internal static string FailedToGetJsonString => @"Failed to get Json string for the key '{0}' from the repository.";
        internal static string JsonConfigurationRepositoryIsNull => @"The provide IJsonConfigurationRepository is null.";
    }
}
