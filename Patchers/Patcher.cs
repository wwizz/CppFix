namespace CppFix
{
    public abstract class Patcher
    {
        public abstract void Patch();
        public string OriginalText { get; }
        public string PatchedText { get; set; }
        public PatchState PatchState { get; set; }
        public string FileName { get; }

        protected Patcher(string originalText, string fileName)
        {
            OriginalText = originalText;
            FileName = fileName;
        }
    }
}