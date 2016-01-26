namespace GateScheduler.Solution
{
    /// <summary>
    /// Stores data as in-memory collections, since persistence is not currently
    /// needed beyond app lifetime.
    /// </summary>
    public class SchedulerRepository
    {
        private Schedule _currentSchedule = new Schedule();

        public Schedule GetSchedule()
        {
            return _currentSchedule.Clone();
        }

        public void SaveSchedule(Schedule schedule)
        {
            _currentSchedule = schedule;
        }
    }
}
