using System;
using Microsoft.Extensions.CommandLineUtils;

namespace CppFix
{
    public class App : CommandLineApplication
    {
        public static void Main(string[] args)
        {
            var app = new App();
            app.Execute(args);
            Console.ReadLine();
        }

        public App()
        {
            Name = "CppFix";
            Commands.Add(new FixGuardsAppCommand());
            HelpOption("-? | -h | --help");
            OnExecute(() =>
            {
                ShowHelp();
                return 1;
            });
        }
    }
}