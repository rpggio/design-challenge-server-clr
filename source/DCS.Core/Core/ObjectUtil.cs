using System;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class ObjectUtil
    {
        public static bool IfType<T>(this object obj, Action<T> action) where T : class
        {
            var objAsT = obj as T;
            if (objAsT != null)
            {
                action(objAsT);
                return true;
            }
            return false;
        }

        public static TResult IfType<T, TResult>(this object obj, Func<T, TResult> action) where T : class
        {
            var objAsT = obj as T;
            if (objAsT != null)
            {
                return action(objAsT);
            }
            return default(TResult);
        }
    }
}