using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class EnumerableUtil
    {
        private static readonly Random _random = new Random();

        public static T SingleRandom<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(_ => _random.Next()).FirstOrDefault();
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        /// <summary>
        /// Expands all instances of the token #{keyName} within the values of the collection,
        /// where keyName is the name of another key within the collection.
        /// </summary>
        /// <returns>New collection with expanded values.</returns>
        public static NameValueCollection ExpandTokens(this NameValueCollection collection)
        {
            var newCollection = new NameValueCollection(collection);
            bool dirty;
            int loops = 0;
            // looping to pick up multiple levels of substitution
            do
            {
                dirty = false;
                foreach (string key in newCollection.AllKeys)
                {
                    string value = newCollection[key];
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    var replaceKeys =
                        Regex.Matches(value, @"\#\{([\w\d\.]+)\}", RegexOptions.IgnoreCase)
                            .Cast<Match>()
                            .Select(m => m.Groups[1].Value)
                            .ToList();
                    if (replaceKeys.Count > 0)
                    {
                        foreach (var replaceKey in replaceKeys)
                        {
                            if (replaceKey.Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                throw new Exception(
                                    "Value {0} is self-referencing".FormatFrom(key));
                            }

                            var replaceValue = newCollection[replaceKey];
                            if (replaceValue == null)
                            {
                                throw new Exception(
                                    "Could not find replacement for {0} in {1}".FormatFrom(
                                        replaceKey,
                                        value));
                            }

                            value = value.Replace("#{" + replaceKey + "}", replaceValue);
                        }
                        newCollection[key] = value;
                        dirty = true;
                    }
                }
                if (++loops > 10)
                {
                    throw new Exception("Unable to complete expansion of collection");
                }
            } while (dirty);
            return newCollection;
        }

        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> items)
        {
            return items.OrderBy(x => _random.Next());
        }
    }
}