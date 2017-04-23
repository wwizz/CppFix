using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CppFix
{
    public class HeaderGuardPatcher : Patcher
    {
        private readonly string _projectPath;
        private const string RegexHeader = @"(?<header>\A\/\*(?:.|\n)*\*\/)?(?:.|\n)*(?<gardbegin>#ifndef.*\n#define.*\n)(?<content>(?:.|\n)*)(?<gardend>#endif.*)\Z";
        private static readonly Regex Regex;

        static HeaderGuardPatcher()
        {
            Regex = new Regex(RegexHeader);
        }

        public HeaderGuardPatcher(string projectPath, string fileName, string originalText, string headerDefinitionText) : base(originalText, fileName)
        {
            _projectPath = projectPath;
            HeaderDefinitionText = headerDefinitionText;
        }

        public string GardEnd => Environment.NewLine + "#endif  // " + Gard;
        public string GardBegin => Environment.NewLine + "#ifdef " + Gard + Environment.NewLine + "#define " + Gard + Environment.NewLine;
        public string Gard => GetHeaderWithoutProjectDir().ToUpper().Replace(Path.DirectorySeparatorChar.ToString(), "_").Replace('.', '_') + "_";

        private string GetHeaderWithoutProjectDir()
        {
            var p = FileName;
            if (_projectPath != "")
                p = FileName.Replace(_projectPath, "");
            p = p.StartsWith('.' + Path.DirectorySeparatorChar.ToString()) ? p.Substring(2) : p;
            p = p.StartsWith(Path.DirectorySeparatorChar.ToString()) ? p.Substring(1) : p;
            return p;
        }

        public string HeaderDefinitionText { get; }

        public override void Patch()
        {
            var match = Regex.Match(OriginalText);
            if (!match.Success)
            {
                PatchState = PatchState.Invalid;
            }
            else
            {
                ConstructNewHeader(match);
                PatchState = PatchedText == OriginalText ? PatchState.NotPatched : PatchState.Patched;
            }
        }

        private void ConstructNewHeader(Match match)
        {
            var content = match.Groups["content"];
            PatchedText = HeaderDefinitionText + GardBegin + content + GardEnd;
        }
    }
}