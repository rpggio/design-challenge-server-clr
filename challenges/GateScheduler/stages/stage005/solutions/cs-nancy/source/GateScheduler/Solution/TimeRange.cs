using System;

namespace GateScheduler.Solution
{
    public struct TimeRange
    {
        private readonly TimeSpan _start;
        private readonly TimeSpan _end;

        public static readonly TimeSpan MinTime = TimeSpan.FromHours(0);
        public static readonly TimeSpan MaxTime = TimeSpan.FromHours(24);

        public TimeRange(TimeSpan start, TimeSpan end)
        {
            if (end < start)
            {
                throw new ArgumentOutOfRangeException("end must be greater than start");
            }

            _start = start;
            _end = end;
        }

        public TimeSpan Start
        {
            get { return _start; }
        }

        public TimeSpan End
        {
            get { return _end; }
        }

        /// <summary>
        /// Is the time within the range, but not equal to an endpoint?
        /// </summary>
        public bool ContainsWithin(TimeSpan time)
        {
            return time > _start && time < _end;
        }

        public bool Overlaps(TimeRange other)
        {
            return ContainsWithin(other.Start)
                   || ContainsWithin(other.End)
                   || other.ContainsWithin(Start)
                   || other.ContainsWithin(End);
        }

        public TimeRange ReduceStart(TimeSpan time)
        {
            TimeSpan start = time > Start ? MinTime : Start - time;
            return new TimeRange(start, End);
        }

        public TimeRange ReduceEnd(TimeSpan time)
        {
            TimeSpan end = MaxTime - End < time ? MaxTime : End + time;
            return new TimeRange(Start, end);
        }
    }
}