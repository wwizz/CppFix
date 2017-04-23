using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CppFix
{
    public class FileRetriever
    {
        private readonly FileFixerOptions _options;

        public FileRetriever(FileFixerOptions options)
        {
            _options = options;
        }

        public List<string> GetHeadersToProcess()
        {
            return GetFiles("*.h")
                .Where(file => IsFileInStartDir(file) &&
                              !IsFileInIgnoreDir(file))
                .ToList();
        }

        public List<string> GetCodefilesToProcess()
        {
            return GetFiles("*.cpp")
                .Where(file => IsFileInStartDir(file) &&
                              !IsFileInIgnoreDir(file))
                .ToList();
        }


        private IEnumerable<string> GetFiles(string mask)
        {
            return Directory.GetFiles(_options.ProjectDir, mask, SearchOption.AllDirectories).Select(GetAbsolutePath);
        }

        private static string GetAbsolutePath(string candidate)
        {
            return Path.GetFullPath(candidate);
        }

        private bool IsFileInIgnoreDir(string file)
        {
            return _options.IgnoreDirs.Any(file.StartsWith);
        }

        private bool IsFileInStartDir(string file)
        {
            return file.StartsWith(_options.StartDir);
        }
    }
}