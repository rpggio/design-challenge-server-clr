using System.Linq;
using Nancy;

namespace GateScheduler.Solution
{
    public class ArrivalsModule : NancyModule
    {
        private readonly Schedule _schedule;

        public ArrivalsModule(Schedule schedule)
            : base("/arrivals")
        {
            _schedule = schedule;

            Get["/"] = _ => _schedule.Flights
                .OrderBy(f => f.Arrives)
                .Select(f =>
                    new
                    {
                        f.Flight,
                        f.Arrives,
                        f.Gate
                    });
        }
    }
}
