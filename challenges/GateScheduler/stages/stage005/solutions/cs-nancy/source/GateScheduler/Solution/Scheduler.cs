using System;
using System.Collections;
using System.Collections.Generic;
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

        public void ScheduleFlights(ICollection<FlightModel> flights)
        {
            foreach (var flight in flights.OrderBy(f => f.Arrives))
            {
                ScheduleFlight(flight);
            }
        }

        public void ScheduleFlight(FlightModel flight)
        {
            _db.AddFlight(flight);
            foreach (var gate in _db.Gates)
            {
                var existingFlights = _db.GetFlightsForGate(gate.Gate);
                if(!existingFlights.Any(f => f == null || !f.CanShareGateWith(flight)))
                {
                    _db.SetFlightGate(flight, gate.Gate);
                    break;
                }
            }
        }
    }
}
