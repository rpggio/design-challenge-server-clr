using System;
using System.Linq.Expressions;

namespace DCS.Core.Core
{
    public static class Require
    {
        public static void ArgumentIs<T>(
            string paramName,
            T value, 
            Predicate<T> requirement,
            string message = "Invalid argument")
        {
            if (!requirement(value))
            {
                throw new ArgumentException(
                    string.Format("{0}: {1}", message, value),
                    paramName);
            }
        }

        // Not working
        //public static void PropertyIsNotNull<T>(T obj, Expression<Func<T>> property) where T : class
        //{
        //    var propertyValue = property.Compile()();
        //    if (propertyValue == null)
        //    {
        //        throw new NullReferenceException(
        //            "Property {0} should not be null".FormatFrom(property.GetPropertyName()));
        //    }
        //}

        public static void PropertyIsNotNull<T>(string propertyName, T value) where T : class
        {
            if (value == null)
            {
                throw new NullReferenceException(
                    "Property {0} should not be null".FormatFrom(propertyName));
            }
        }
    }
}
