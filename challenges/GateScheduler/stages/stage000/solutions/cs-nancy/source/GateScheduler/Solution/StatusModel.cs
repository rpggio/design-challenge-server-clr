namespace GateScheduler.Solution
{
    public class StatusModel
    {
        public string StartupKey { get; set; }

        public SystemState State { get; set; }
    }

    public enum SystemState
    {
        Invalid,
        Ready,
        Reset
    }
}