using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class FuncUtil
    {
        public static bool TryExecute(Action action)
        {
            try
            {
                action();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static TResult TryExecute<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch
            {
                return default(TResult);
            }
        }

        public static bool TryExecute<TResult>(Func<TResult> func, out TResult result)
        {
            try
            {
                result = func();
                return true;
            }
            catch
            {
                result = default(TResult);
                return false;
            }
        }
    }
}