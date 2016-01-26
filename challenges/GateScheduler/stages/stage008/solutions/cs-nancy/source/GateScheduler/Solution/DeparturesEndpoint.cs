using System.Linq;
using Nancy;

namespace GateScheduler.Solution
{
    public class DeparturesEndpoint : NancyModule
    {
        private readonly Schedule _schedule;

        public DeparturesEndpoint(Schedule schedule)
            : base("/departures")
        {
            _schedule = schedule;

            Get["/"] = _ => _schedule.Flights
                .OrderBy(f => f.Departs)
                .Select(f =>
                    new
                    {
                        f.Flight,
                        f.Departs,
                        f.Gate
                    });
        }
    }
}
