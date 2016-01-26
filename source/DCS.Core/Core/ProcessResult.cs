using System;

// ReSharper disable once CheckNamespace
namespace DCS.Core
{
    public class ProcessResult
    {
        public ProcessResult(int exitCode)
        {
            ExitCode = exitCode;
        }

        public int ExitCode { get; private set; }

        public string StandardOutput { get; set; }
        
        public string StandardError { get; set; }

        public string AllOutput { get; set; }

        public string OutputSummary
        {
            get
            {
                string output = StandardOutput.Left(2000);
                if (StandardError.IsEmpty())
                {
                    return output;
                }
                return StandardError.Left(2000)
                       + Environment.NewLine
                       + output;
            }
        }
    }
}