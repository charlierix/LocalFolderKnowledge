using LocalFolderKnowledge.ClassLib.Models;

namespace LocalFolderKnowledge.ClassLib.Implementations
{
    public static class AddFolder
    {
        public static AddFolderResponse Add(AddFolderRequest request)
        {
            return new AddFolderResponse
            {
                IsSuccess = false,
                ErrorMessage = "FINISH THIS",
            };
        }
    }
}
