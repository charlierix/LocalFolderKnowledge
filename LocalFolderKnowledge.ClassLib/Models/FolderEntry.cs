namespace LocalFolderKnowledge.ClassLib.Models
{
    public record FolderEntry
    {
        // Unique name of the entry
        public string Name { get; init; }

        public string OriginalSourceFolder { get; init; }
        public string LiveSourceFolder { get; init; }
    }
}
