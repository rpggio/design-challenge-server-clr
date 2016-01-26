using System;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class PrimitiveUtil
    {
        public static string Abbreviate(this Guid guid)
        {
            return guid.ToString("N").Substring(0, 8);
        }

        public static void RequireTrue(this bool value, string message = null)
        {
            if (!value)
            {
                throw new Exception(message ?? "Value should have been true");
            }
        }

        public static bool IsDefault<T>(this T value) where T : IEquatable<T>
        {
            return default(T).Equals(value);
        }

        public static Guid? ToNullableGuid(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }
            Guid result;
            return Guid.TryParse(value, out result)
                ? result
                : (Guid?)null;
        }
    }
}
