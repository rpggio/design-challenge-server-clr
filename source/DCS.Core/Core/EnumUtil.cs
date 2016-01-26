using System;

namespace DCS.Core.Core
{
    public static class EnumUtil
    {
        public static bool HasFlagValue<T>(this T value, T checkValue)
            where T : struct, IComparable
        {
            Require.ArgumentIs("T", typeof(T), t => t.IsEnum,
                "must be Enum type");
            return ((Enum) (object) value).HasFlag((Enum) (object) checkValue);
        }
    }
}
