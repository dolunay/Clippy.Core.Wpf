namespace Wpf.Clippy.Types
{
    public sealed class SpeechLanguage
    {
        internal SpeechLanguage(string cultureName, string displayName, string voiceName)
        {
            CultureName = cultureName;
            DisplayName = displayName;
            VoiceName = voiceName;
        }

        public string CultureName { get; }
        public string DisplayName { get; }
        public string VoiceName { get; }

        public override string ToString() => DisplayName;
    }
}