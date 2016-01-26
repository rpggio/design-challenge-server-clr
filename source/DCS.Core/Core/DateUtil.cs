using System;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public static class DateUtil
    {
        public static DateTime? TryParse(string dateString)
        {
            if (string.IsNullOrEmpty(dateString))
            {
                return null;
            }
            DateTime result;
            if (!DateTime.TryParse(dateString, out result))
            {
                return null;
            }
            return result;
        }
    }
}
