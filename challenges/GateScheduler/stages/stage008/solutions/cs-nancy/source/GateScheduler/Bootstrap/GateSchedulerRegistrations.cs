using GateScheduler.Solution;
using Nancy.TinyIoc;

namespace GateScheduler.Bootstrap
{
    /// <summary>
    /// The type registrations for the application.
    /// </summary>
    public class GateSchedulerRegistrations
    {
        public static void Register(TinyIoCContainer container)
        {
            // All dependencies on SchedulerDatabase receive the same singleton instance.
            container.Register<SchedulerRepository>().AsSingleton();
            container.Register((a, b) => a.Resolve<SchedulerRepository>().GetSchedule());

            // TinyIoC automatically registers types as multi-intance, 
            // so you only have to register them here if you need to change
            // the default registration.
        }
    }
}
