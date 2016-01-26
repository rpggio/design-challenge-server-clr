namespace DCS.Contracts
{
    public enum AssessmentOutcome
    {
        Unknown = 0,
        Success,
        InvalidContent,
        BuildFailure,
        SolutionFailure,
        TestFailure,
        SystemBusy
    }
}