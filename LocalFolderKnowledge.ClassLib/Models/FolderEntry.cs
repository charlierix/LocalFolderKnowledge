namespace LocalFolderKnowledge.ClassLib.Models
{
    public record FolderEntry
    {
        public const string COPY_FOLDER = "copy";
        public const string RAG_FOLDER = "raganything";

        // Unique name of the entry
        public string Name { get; init; }

        public string OriginalSourceFolder { get; init; }

        // This is the sub folder under appsettings.FolderLocation (not full path, just subfolder)
        public string LiveSourceFolder { get; init; }

        public bool IsCopy { get; init; }
        public string? CopyFolder => IsCopy ? COPY_FOLDER : null;

        // When a folder is first added, it takes a while to parse it.  This will be true when that is finished and
        // it's ready to be used
        public bool IsFinishedParsing { get; init; }
    }
}
