using System;
using Nancy;
using Nancy.ModelBinding;

namespace GateScheduler.Solution
{
    public class StatusEndpoint : NancyModule
    {
        public StatusEndpoint(Scheduler scheduler, SchedulerDatabase db)
            : base("/status")
        {
            Get["/"] = _ =>
                new StatusModel
                {
                    State = scheduler.State
                };

            Put["/"] = _ =>
            {
                var status = this.Bind<StatusModel>();
                
                if (status.State == SystemState.ClearData)
                {
                    db.ClearAll();
                }
                else if (scheduler.State != status.State)
                {
                    Console.WriteLine("System status set to {0}", status.State);
                    scheduler.State = status.State;
                }

                return new StatusModel
                {
                    UserKey = Program.UserKey,
                    State = scheduler.State
                };
            };
        }
    }
}
