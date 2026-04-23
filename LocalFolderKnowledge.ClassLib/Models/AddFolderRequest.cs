namespace LocalFolderKnowledge.ClassLib.Models
{
    public record AddFolderRequest
    {
        // What to call this entry
        // NOTE: if this is blank, the SourceFolder's name will be used
        // NOTE: if this isn't unique, a number suffix will be added to make it unique (see the response object for what it was called)
        public string Name { get; init; }

        public string SourceFolder { get; init; }

        // For now, only allow copy
        // True: SourceFolder will be copied to local knowledge folder.  This creates a frozen snapshot
        // False: Will link to the source folder.  If that folder is removed in the future, tool calls will be limited
        //public bool ShouldCopyContents { get; init; }
        public bool ShouldCopyContents => true;
    }
}
