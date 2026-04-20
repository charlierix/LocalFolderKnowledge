namespace LocalFolderKnowledge.ClassLib.Models
{
    public record AddFolderRequest
    {
        // What to call this entry
        // NOTE: if this isn't unique, a number suffix will be added to make it unique (see the response object for what it was called)
        public string Name { get; init; }
    }
}
