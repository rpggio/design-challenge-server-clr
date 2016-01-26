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

        public bool CanShareGateWith(FlightModel flight)
        {
            return Range.DistanceFrom(flight.Range) >= TimeSpan.FromMinutes(30);
        }
    }
}