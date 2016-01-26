using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DCS.Core;
using Xunit;

namespace DCS.UnitTests
{
    public class SolutionRunTests
    {
        // Use this in GateScheduler to generate a lot of output.
        //Console.WriteLine(string.Join(",", Enumerable.Range(1, 20).Select(_ => Guid.NewGuid().ToString())));
        //var rand = new Random();
        //for (int i = 0; i < 10000; i++)
        //{
        //    Console.WriteLine(i + ": " + string.Join(",", Enumerable.Range(1, 20).Select(_ => Guid.NewGuid().ToString())));
        //    Console.Error.WriteLine("Errorrr: " + i);
        //    Thread.Sleep(50);
        //}
        [Fact]
        public void Run_a_process_task_newest()
        {
            string exe =
                @"C:\work\me\design-challenge-server\challenges\GateScheduler\stages\stage000\solutions\cs-nancy\source\GateScheduler\bin\debug\GateScheduler.exe";

            var outputCompletion = new TaskCompletionSource<string>();
            bool setResult = false;
            Action<string> outputAction = s =>
            {
                if (!setResult)
                {
                    setResult = true;
                    outputCompletion.SetResult(s);
                }
            };
            var outputTask = outputCompletion.Task;

            var solutionRun = ProcessRun.Start(new ProcessStartInfo(exe), outputAction);

            int waitResult = Task.WaitAny(new Task[] { outputTask, solutionRun.Task }, TimeSpan.FromSeconds(5));

            System.Console.WriteLine("Wait result: " + waitResult);

            if (outputTask.IsCompleted)
            {
                System.Console.WriteLine("Output: " + outputTask.Result);
            }

            // pretend tests are running
            Thread.Sleep(500);

            if (!solutionRun.Process.HasExited)
            {
                solutionRun.Process.Kill();
            }

            solutionRun.Task.Wait(1000);

            if (solutionRun.Task.IsCompleted)
            {
                System.Console.WriteLine("-- output --");
                System.Console.WriteLine(solutionRun.Task.Result.StandardOutput);
                
                System.Console.WriteLine("-- error --");
                System.Console.WriteLine(solutionRun.Task.Result.StandardError);
            }
            else
            {
                System.Console.WriteLine("solution task not completed!");
            }
        }
    }
}