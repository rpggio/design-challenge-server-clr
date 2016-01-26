using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    public class StatusEndpoint : NancyModule
    {
        public StatusEndpoint(SchedulerRepository repository)
            : base("/status")
        {
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
                    repository.SaveSchedule(new Schedule());
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
