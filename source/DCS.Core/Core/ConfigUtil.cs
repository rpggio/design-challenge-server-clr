using System;
using System.Collections.Specialized;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class ConfigUtil
    {
        public static string GetRequiredValue(this NameValueCollection collection, string name)
        {
            var value = collection[name];
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Missing required key '{0}'".FormatFrom(name));
            }
            return value;
        }
    }
}
