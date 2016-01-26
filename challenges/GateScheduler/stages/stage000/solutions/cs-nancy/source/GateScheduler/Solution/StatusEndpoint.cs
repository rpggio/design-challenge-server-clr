using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    public class StatusEndpoint : NancyModule
    {
        private readonly SchedulerDatabase _db;

        public StatusEndpoint(SchedulerDatabase db)
            : base("/status")
        {
            _db = db;

            Get["/"] = _ =>
                new StatusModel
                {
                    StartupKey = Program.StartupKey,
                    State = SystemState.Ready
                };

            Put["/"] = _ =>
            {
                var status = this.Bind<StatusModel>();
                if (status.State == SystemState.Reset)
                {
                    _db.ClearAll();
                }
                return new StatusModel
                {
                    StartupKey = Program.StartupKey,
                    State = SystemState.Ready
                };
            };
        }
    }
}
