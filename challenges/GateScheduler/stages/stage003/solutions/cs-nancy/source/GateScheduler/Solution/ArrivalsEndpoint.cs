using System.Linq;
using Nancy;

namespace GateScheduler.Solution
{
    public class ArrivalsModule : NancyModule
    {
        private readonly SchedulerDatabase _db;

        public ArrivalsModule(SchedulerDatabase db)
            : base("/arrivals")
        {
            _db = db;

            Get["/"] = _ => _db.Flights
                .OrderBy(f => f.Arrives)
                .Select(f =>
                    new
                    {
                        f.Flight,
                        f.Arrives,
                        _db.GetGateOrNullForFlight(f.Flight).Gate
                    });
        }
    }
}
