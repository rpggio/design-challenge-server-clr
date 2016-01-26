using System.CodeDom.Compiler;

namespace DCS.Console.Commands
{
    public interface IConsoleCommand
    {
        bool TryParse(string[] args, out string message);
        void PrintHelp(IndentedTextWriter writer);
        bool Execute();
    }
}