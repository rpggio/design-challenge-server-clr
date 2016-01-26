using System;
using System.Linq.Expressions;

namespace DCS.Core.Core
{
    public static class ReflectionUtil
    {
        public static string GetPropertyName<T>(this Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;
            return body.Member.Name;
        }
    }
}
