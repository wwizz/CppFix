using System;
using Microsoft.Extensions.CommandLineUtils;

namespace CppFix
{
    public abstract class AppCommand : CommandLineApplication
    {
        protected string CommandName;
        public abstract int Execute();

        protected AppCommand()
        {
            HelpOption("-? | -h | --help");
            OnExecute(() => Execute());
        }

        protected void LogArgument(CommandArgument arg)
        {
            if (arg.MultipleValues)
            {
                LogMultiValueArgument(arg);
            }
            else
            {
                LogSingleValueArgument(arg);
            }
        }

        protected void LogSingleValueArgument(CommandArgument arg)
        {
            Console.WriteLine(arg.Name + ": " + arg.Value);
        }

        protected static void LogMultiValueArgument(CommandArgument arg)
        {
            Console.WriteLine(arg.Name + ": " + arg.Value);
            foreach (var v in arg.Values)
            {
                Console.WriteLine("  " + v);
            }
        }

        protected bool IsArgumentSet(CommandArgument arg)
        {
            if (arg.Value != null)
                return true;

            Console.WriteLine(arg.Name + " not set");
            ShowHelp(CommandName);
            Environment.ExitCode = 1;
            return false;
        }

        
    }
}