using System;
using Microsoft.Extensions.CommandLineUtils;

namespace CppFix
{
    public class FixGuardsAppCommand : AppCommand
    {
        private readonly CommandArgument _headerDefinitionPath;
        private readonly CommandArgument _projectDir;
        private readonly CommandArgument _startDir;
        private readonly CommandArgument _ignoreDirs;

        public FixGuardsAppCommand()
        {
            CommandName = "CppFix fix-headers";
            Name = "fix-guards";
            _headerDefinitionPath = Argument("header-def-path", "Enter the path to the header definition file.");
            _projectDir = Argument("project-dir","Enter the path of the project directory.");
            _startDir = Argument("start-dir", "Enter the path to start from");
            _ignoreDirs = Argument("ignore-dirs", "Enter the path to ignore", true);
        }

        public override int Execute()
        {
            if (!IsArgumentSet(_headerDefinitionPath))
                return 1;
            if (!IsArgumentSet(_projectDir))
                return 1;
            if (!IsArgumentSet(_startDir))
                return 1;

            LogArgument(_headerDefinitionPath);
            LogArgument(_projectDir);
            LogArgument(_startDir);
            LogArgument(_ignoreDirs);

            var options = new FileFixerOptions
            {
                ProjectDir = _projectDir.Value,
                StartDir = _startDir.Value,
                HeaderDefinitionFile = _headerDefinitionPath.Value,
                IgnoreDirs = _ignoreDirs.Values,
                UseBackup = false
            };

            var fix = new FileFixer(options);

            Console.WriteLine();
            Console.WriteLine("Modified:");
            foreach (var header in fix.ModifiedFiles)
            {
                Console.WriteLine(header);
            }

            Console.WriteLine();
            Console.WriteLine("Unmodified:");
            foreach (var header in fix.UnmodifiedFiles)
            {
                Console.WriteLine(header);
            }

            Console.WriteLine();
            Console.WriteLine("Invalid:");
            foreach (var header in fix.InvalidFiles)
            {
                Console.WriteLine(header);
            }
            return 0;
        }
    }
}