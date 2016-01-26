using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace GateScheduler.FeatureTests
{
    /// <summary>
    /// This executes the Cucumber feature tests by invoking Ruby.
    /// You will need to fist install Ruby 1.9.3 and include it on PATH (version 1.9.3 at http://rubyinstaller.org/downloads/).
    /// Bundler will also need to be installed (http://bundler.io/).
    /// </summary>
    static internal class TestExecution
    {
        internal static bool RunTests()
        {
            Console.WriteLine("This will run the feature tests with Cucumber in Ruby");
            Console.WriteLine();

            string repoDirectory = Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
            // move upward to find directory containing 'features\*.feature'
            while (
                !Directory.Exists(Path.Combine(repoDirectory, "features"))
                || !Directory.EnumerateFiles(
                    Path.Combine(repoDirectory, "features"), "*.feature")
                    .Any()
                )
            {
                repoDirectory = Path.GetDirectoryName(repoDirectory);
                if (repoDirectory == null)
                {
                    Console.WriteLine("Could not find an ancestor to {0} containing dir 'features'",
                        AppDomain.CurrentDomain.BaseDirectory);
                    return false;
                }
            }

            if (!CheckRubyAvailable())
            {
                Console.WriteLine("Ruby needs to be installed and available from the PATH");
                Console.WriteLine();
                return false;
            }

            // wait for solution app to start up
            Thread.Sleep(300);

            if (!CheckSolutionRunning())
            {
                Console.WriteLine("The solution app is not running or could not connect.");
                Console.WriteLine("To run both the solution and this test app at the same time, set both projects to start up. This is done by editing properties on the Solution in the Solution Explorer (right-click).");
                return false;
            }

            if (Shell.ExecuteBackround("cmd", "/c bundle exec cucumber --strict", repoDirectory) == 0)
            {
                Console.WriteLine();
                Console.WriteLine("---------- PASS ----------");
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            Console.WriteLine("---------- FAIL ----------");
            Console.WriteLine();
            return false;
        }

        private static bool CheckSolutionRunning()
        {
            using (var client = new TcpClient())
            {
                try
                {
                    client.Connect("localhost", GateScheduler.Program.TestingPort);
                    return true;
                }
                catch (SocketException)
                {
                    return false;
                }
            }
        }

        private static bool CheckRubyAvailable()
        {
            return Shell.ExecuteBackround("ruby", "--version") == 0;
        }
    }
}