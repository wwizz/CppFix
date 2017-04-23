using System;
using System.Collections.Generic;
using System.IO;

namespace CppFix
{
    public class FileFixer
    {
        private readonly FileFixerOptions _options;

        public FileFixer(FileFixerOptions options)
        {
            _options = options;

            MakeAbsolutePaths();

            ModifiedFiles = new List<string>();
            UnmodifiedFiles = new List<string>();
            InvalidFiles = new List<string>();

            ProcessFiles();
        }

        private void ProcessFiles()
        {
            ProcessHeaderFiles();
            ProcessCodeFiles();
        }

        private void ProcessCodeFiles()
        {
            CodeFiles = new FileRetriever(_options).GetCodefilesToProcess();
            foreach (var file in CodeFiles)
            {
                PatchFile(GetPatcherForCodeFile(file));
            }
        }

        private void ProcessHeaderFiles()
        {
            Headers = new FileRetriever(_options).GetHeadersToProcess();
            foreach (var file in Headers)
            {
                PatchFile(GetPatcherForHeaderFile(file));
            }
        }

        private Patcher GetPatcherForCodeFile(string fileName)
        {
            switch (_options.Action)
            {
                case FileFixerAction.Namespace:
                    return new NamespacePatcher(File.ReadAllText(fileName), fileName);
            }
            return null;
        }


        private void PatchFile(Patcher patcher)
        {
            patcher.Patch();

            switch (patcher.PatchState)
            {
                case PatchState.Invalid:
                    AddToInvalidFiles(patcher.FileName);
                    break;
                case PatchState.Patched:
                    AddToModifiedFiles(patcher.FileName);
                    WriteHeader(patcher.FileName, patcher.OriginalText, patcher.PatchedText);
                    break;
                case PatchState.NotPatched:
                    AddToUnmodifiedFiles(patcher.FileName);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private Patcher GetPatcherForHeaderFile(string fileName)
        {
            switch (_options.Action)
            {
                case FileFixerAction.HeaderGuard:
                    ReadHeaderDefinitionFile();
                    return new HeaderGuardPatcher(_options.ProjectDir, fileName, File.ReadAllText(fileName), HeaderDefinitionText);
                case FileFixerAction.Namespace:
                    return new NamespacePatcher(File.ReadAllText(fileName), fileName);
                default:
                    return null;
            }
        }

        private void AddToUnmodifiedFiles(string fileName)
        {
            UnmodifiedFiles.Add(fileName);
        }

        private void AddToModifiedFiles(string fileName)
        {
            ModifiedFiles.Add(fileName);
        }

        private void AddToInvalidFiles(string fileName)
        {
            InvalidFiles.Add(fileName);
        }

        private void WriteHeader(string header, string headerText, string patchedText)
        {
            if (_options.UseBackup)
            {
                File.WriteAllText(header + ".bak", headerText);
            }
            File.WriteAllText(header, patchedText);
        }

        private void MakeAbsolutePaths()
        {
            
            _options.ProjectDir = MakeFullPath(_options.ProjectDir);
            _options.StartDir = MakeFullPath(_options.StartDir);

            for (var index = 0; index < _options.IgnoreDirs.Count; index++)
            {
                var optionsIgnoreDir = _options.IgnoreDirs[index];
                _options.IgnoreDirs[index] =  MakeFullPath(optionsIgnoreDir);
            }
        }

        private static string MakeFullPath(string dir)
        {
            if (string.IsNullOrEmpty(dir))
                dir = ".";
            return Path.GetFullPath(dir);
        }

        private void ReadHeaderDefinitionFile()
        {
            if (!File.Exists(_options.HeaderDefinitionFile))
            {
                throw new Exception("Error headerfile: " + _options.HeaderDefinitionFile + " does not exits");
            }

            HeaderDefinitionText = File.ReadAllText(_options.HeaderDefinitionFile);
        }

        public List<string> UnmodifiedFiles { get; set; }
        public List<string> ModifiedFiles { get; set; }
        public string HeaderDefinitionText { get; set; }
        public List<string> Headers { get; set; }
        public List<string> InvalidFiles { get; set; }
        public List<string> CodeFiles { get; set; }
    }
}
