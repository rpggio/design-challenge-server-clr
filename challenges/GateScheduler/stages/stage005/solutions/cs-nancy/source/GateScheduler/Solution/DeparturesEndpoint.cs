using System.Linq;
using Nancy;

namespace GateScheduler.Solution
{
    public class DeparturesEndpoint : NancyModule
    {
        private readonly SchedulerDatabase _db;

        public DeparturesEndpoint(SchedulerDatabase db)
            : base("/departures")
        {
            _db = db;

            Get["/"] = _ => _db.Flights
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
