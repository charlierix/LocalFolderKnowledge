using LocalFolderKnowledge.ClassLib.Models;

namespace LocalFolderKnowledge.ClassLib.Implementations
{
    public static class AddFolder
    {
        public static AddFolderResponse Add(AddFolderRequest request)
        {
            try
            {
                return new AddFolderResponse
                {
                    IsSuccess = false,
                    ErrorMessage = "FINISH THIS",
                };
            }
            catch (Exception ex)
            {
                return new AddFolderResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"Caught exception: {ex.Message}",
                };
            }
        }
    }
}
