using DCS.Contracts;

namespace DCS.ServerRuntime.Services
{
    public class AssessmentResult
    {
        public AssessmentResult(AssessmentOutcome outcome)
        {
            Outcome = outcome;
        }

        public AssessmentOutcome Outcome { get; private set; }
        public string Message { get; set; }
        public string BuildOutput { get; set; }
        public TestOutputFormat TestOutputFormat { get; set; }
        public string TestOutput { get; set; }
    }
}