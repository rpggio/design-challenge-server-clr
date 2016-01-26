using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class StringUtil
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string a, string b)
        {
            if (a == null || b == null)
            {
                return false;
            }
            return a.ToLowerInvariant().Contains(b.ToLowerInvariant());
        }

        public static string ToStringInvariant(this int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToStringInvariant(this double value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string Left(this string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }
            return value;
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static string FormatFrom(this string format, params object[] values)
        {
            return string.Format(format, values);
        }

        public static string ToStringOrNull(this object obj)
        {
            return obj == null
                ? null
                : obj.ToString();
        }

        public static string JoinString(this IEnumerable<string> items, string delimiter)
        {
            return string.Join(delimiter, items);
        }

        public static string EnsureSurrounded(this string value, string surroundWith)
        {
            if(!(value.StartsWith(surroundWith) && value.EndsWith(surroundWith)))
            {
                return surroundWith + value + surroundWith;
            }
            return value;
        }

        public static string EnsureEndsWith(this string value, string suffix)
        {
            if (value == null)
            {
                return null;
            }
            if (!value.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return value + suffix;
            }
            return value;
        }

        public static string TrimRight(this string value, int trimLength)
        {
            if (value.IsEmpty())
            {
                return value;
            }

            if (trimLength >= value.Length)
            {
                return string.Empty;
            }

            return value.Substring(0, value.Length - trimLength);
        }
    }
}