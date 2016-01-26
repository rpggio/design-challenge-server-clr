using System;
using System.Linq;

namespace GateScheduler.Solution
{
    /// <summary>
    /// Placeholder, provided as an example of a
    /// class with dependency on SchedulerDatabase.
    /// The (singleton) SchedulerDatabase will automatically 
    /// be injected by the framework.
    /// </summary>
    public class Scheduler
    {
        private readonly SchedulerDatabase _db;

        public Scheduler(SchedulerDatabase db)
        {
            _db = db;
        }

        public void ScheduleFlight(FlightModel flight)
        {
            foreach (var gate in _db.Gates)
            {
                var existingFlights = _db.GetFlightsForGate(gate.Gate).ToList();
                if(!existingFlights.Any(ef => Math.Abs((ef.Arrives - flight.Arrives).TotalHours) < 1))
                {
                    _db.SetFlightGate(flight.Flight, gate.Gate);
                    break;
                }
            }
        }
    }
}
