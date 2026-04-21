namespace LocalFolderKnowledge.ClassLib.Models
{
    /// <summary>
    /// This will be a settings.json in each of the subfolders
    /// </summary>
    public record EntrySettings
    {
        public const string SETTINGS_FILENAME = "settings.json";

        public FolderEntry Entry { get; init; }

        // add any extra settings that may be needed by the tools
    }
}
