using System.Collections.Generic;

namespace CppFix
{
    public enum FileFixerAction
    {
        HeaderGuard,
        Namespace
    }

    public class FileFixerOptions
    {
        public bool UseBackup { get; set; }
        public string HeaderDefinitionFile { get; set; }
        public string ProjectDir { get; set; }
        public string StartDir { get; set; }
        public List<string> IgnoreDirs { get; set; }
        public FileFixerAction Action { get; set; }


        public FileFixerOptions()
        {
            UseBackup = false;
            HeaderDefinitionFile = "header.txt";
            ProjectDir = ".";
            IgnoreDirs = new List<string>();
        }
    }
}