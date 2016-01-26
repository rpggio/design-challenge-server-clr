using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    /// <summary>
    ///     Nancy module for serving /flights endpoint.
    ///     See general comments in GatesModule.
    /// </summary>
    public class FlightsModule : NancyModule
    {
        public FlightsModule(SchedulerRepository repository)
            : base("/flights")
        {
            // Get all flights
            Get["/"] = _ => repository.GetSchedule().Flights
                .Select(f => new
                {
                    f.Flight,
                    f.Arrives,
                    f.Departs
                });

            // Get all flights
            Get["/unscheduled"] = _ => repository.GetSchedule().Flights
                .Where(f => f.Gate == null)
                .Select(f => new
                {
                    f.Flight,
                    f.Arrives,
                    f.Departs
                });

            // Add flights
            Post["/"] = parameters =>
            {
                // This API POSTs a list of flights at a time instead of individual flights.
                var flights = this.Bind<IEnumerable<FlightModel>>().ToList();
                var schedule = repository.GetSchedule();
                schedule.AddFlights(flights);
                schedule = Schedule.ScheduleUnassignedFlights(schedule);
                repository.SaveSchedule(schedule);
                return null;
            };
        }
    }
}