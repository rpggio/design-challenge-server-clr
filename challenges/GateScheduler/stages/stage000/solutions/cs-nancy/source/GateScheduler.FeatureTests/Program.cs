using System;

namespace GateScheduler.FeatureTests
{
    static class Program
    {
        static int Main()
        {
            try
            {
                if (TestExecution.RunTests())
                {
                    return 0;
                }
                return 1;
            }
            finally
            {
                Console.WriteLine("Hit enter to close");
                Console.ReadLine();
            }
        }
    }
}
