using System;

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
            // _schedulerDatabase.SetGateForFlight(flight, gate) ...
        }
    }
}
