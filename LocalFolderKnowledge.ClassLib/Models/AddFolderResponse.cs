namespace LocalFolderKnowledge.ClassLib.Models
{
    public record AddFolderResponse
    {
        public bool IsSuccess { get; init; }
        public string ErrorMessage { get; init; }
        public FolderEntry Entry { get; init; }
    }
}
