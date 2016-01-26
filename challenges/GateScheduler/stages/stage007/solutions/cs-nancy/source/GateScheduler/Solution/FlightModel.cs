using System;

namespace GateScheduler.Solution
{
    public class FlightModel
    {
        public string Flight { get; set; }

        /// <summary>
        /// Arrival times are represented as hh:mm
        /// </summary>
        public TimeSpan Arrives { get; set; }

        /// <summary>
        /// Departure times are represented as hh:mm
        /// </summary>
        public TimeSpan Departs { get; set; }

        public string Gate { get; set; }

        public TimeRange Range
        {
            get {  return new TimeRange(Arrives, Departs); }
        }

        public bool CanLoadAtSameGateAs(FlightModel flight)
        {
            return Range.DistanceFrom(flight.Range) >= TimeSpan.FromMinutes(30);
        }

        public bool CanLoadAtGateNextTo(FlightModel flight)
        {
            TimeSpan minDist = TimeSpan.FromMinutes(10);
            return Arrives.DistanceFrom(flight.Arrives) >= minDist
                   && Arrives.DistanceFrom(flight.Departs) >= minDist
                   && Departs.DistanceFrom(flight.Arrives) >= minDist
                   && Departs.DistanceFrom(flight.Departs) >= minDist;
        }
    }
}