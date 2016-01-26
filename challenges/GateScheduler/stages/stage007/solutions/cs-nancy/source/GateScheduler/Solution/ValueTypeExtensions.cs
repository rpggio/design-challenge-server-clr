using System;

namespace GateScheduler.Solution
{
    public static class ValueTypeExtensions
    {
        /// <summary>
        /// Return absolute value of difference of two times.
        /// </summary>
        public static TimeSpan DistanceFrom(this TimeSpan a, TimeSpan b)
        {
            return new TimeSpan(Math.Abs(a.Ticks - b.Ticks));
        }
    }
}
