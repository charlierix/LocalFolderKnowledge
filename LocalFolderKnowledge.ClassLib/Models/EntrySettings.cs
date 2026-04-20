namespace LocalFolderKnowledge.ClassLib.Models
{
    /// <summary>
    /// This will be a settings.json in each of the subfolders
    /// </summary>
    public record EntrySettings
    {
        public FolderEntry Entry { get; init; }

        // add any extra settings that may be needed by the tools
    }
}
