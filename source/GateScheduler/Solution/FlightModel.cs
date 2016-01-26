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
    }
}