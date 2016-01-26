using System;
using System.CodeDom.Compiler;
using DCS.Core;

namespace DCS.Console.Commands
{
    public abstract class ConsoleCommandBase : IConsoleCommand
    {
        public bool TryParse(string[] args, out string message)
        {
            try
            {
                ArgsUtil.BindModel(this, args);
            }
            catch (InvalidOperationException ex)
            {
                message = ex.Summary();
                return false;
            }
            message = null;
            return true;
        }

        public void PrintHelp(IndentedTextWriter writer)
        {
            ArgsUtil.WriteHelp(GetType(), writer);
        }

        public abstract bool Execute();
    }
}