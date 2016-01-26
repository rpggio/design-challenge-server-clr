using System;
using System.CodeDom.Compiler;
using System.ComponentModel.DataAnnotations;
using Args;
using DCS.Core;
using DCS.Core.IO;
using DCS.ServerRuntime.Services;
using ServiceStack.Text;

namespace DCS.Console.Commands
{
    public class RunSolutionCommand : IConsoleCommand
    {
        private readonly SolutionRunner _solutionRunner;

        public RunSolutionCommand(SolutionRunner solutionRunner)
        {
            _solutionRunner = solutionRunner;
        }

        [Required]
        public string Exe { get; set; }

        [Required]
        public string TestDir { get; set; }

        public bool TryParse(string[] args, out string message)
        {
            try
            {
                Configuration.Configure<RunSolutionCommand>().BindModel(this, args);
                message = null;
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Summary();
                return false;
            }
        }

        public void PrintHelp(IndentedTextWriter writer)
        {
            writer.WriteLine("runSolution /exe <executable> /testDir <testDir>");
        }

        public bool Execute()
        {
            var result = _solutionRunner.Run(new PathFile(Exe), new PathDirectory(TestDir));
            result.PrintDump();
            return true;
        }
    }
}