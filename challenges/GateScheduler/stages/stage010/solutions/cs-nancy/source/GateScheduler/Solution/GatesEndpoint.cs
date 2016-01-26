using System.Collections.Generic;
using System.Linq;
using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    /// <summary>
    /// Nancy web server module for handling requests to /gates endpoint.
    /// </summary>
    public class GatesModule : NancyModule
    {
        /// <summary>
        /// Create Gates Module. The constructor is called by the IoC container
        /// instead of by our own code.
        /// </summary>
        public GatesModule(SchedulerRepository repository)
            : base("/gates")
        {
            // This tells the web server to use the provided delegate (Lambda)
            // to serve a GET request at this endpoint.
            Get["/"] = _ => repository.GetSchedule().Gates
                .Select(g => new
                {
                    g.Gate
                });

            Post["/"] = _ =>
            {
                var gates = this.Bind<IEnumerable<GateModel>>();
                var schedule = repository.GetSchedule();
                schedule.AddGates(gates.ToArray());
                schedule.ScheduleUnassignedFlights();
                repository.SaveSchedule(schedule);
                return null;
            };
        }
    }
}
