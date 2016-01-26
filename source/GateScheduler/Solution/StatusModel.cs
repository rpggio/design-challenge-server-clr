namespace GateScheduler.Solution
{
    public class StatusModel
    {
        public string UserKey { get; set; }

        public bool? Passed { get; set; }

        public SystemState State { get; set; }
    }

    public enum SystemState
    {
        Invalid,
        Ready,
        Running,
        ClearData,
        Finished
    }
}