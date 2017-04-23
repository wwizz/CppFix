using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CppFix
{
    public class NamespacePatcher: Patcher
    {
        private const string RegexDef = @"(?<begincontent>(?:\n|.)*)namespace\s+(?<namespace>.*)(?:\n|\s)*(?<bracketopen>{)(?<midcontent>(?:\n|.)*)(?<bracketclose>}).*\n(?<endcontent>(?:\n|.)*)\Z";
        private static readonly Regex Regex;

        static NamespacePatcher()
        {
            Regex = new Regex(RegexDef);
        }


        public NamespacePatcher(string originalText, string fileName) : base(originalText, fileName)
        {
        }

        public override void Patch()
        {
            var match = Regex.Match(OriginalText);
            if (!match.Success)
                PatchState = PatchState.Invalid;
            ConstructNewHeader(match);
            PatchState = PatchedText == OriginalText ? PatchState.NotPatched : PatchState.Patched;
        }

        private void ConstructNewHeader(Match match)
        {
            var begincontent = match.Groups["begincontent"];
            var namespacen = match.Groups["namespace"];
            var midcontent = match.Groups["midcontent"];
            var endcontent = match.Groups["endcontent"];
            var namespbegin = Environment.NewLine + "namespace " + namespacen + "{" + Environment.NewLine;
            var namespend = Environment.NewLine + "}  // " + namespacen + Environment.NewLine;

            PatchedText = begincontent +  namespbegin + midcontent + namespend + endcontent;
        }
    }
}