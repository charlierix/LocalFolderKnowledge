using LocalFolderKnowledge.ClassLib.Models;

namespace LocalFolderKnowledge.ClassLib.Implementations
{
    public static class AddFolder
    {
        public static AddFolderResponse Add(AddFolderRequest request, string folderLocation)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderLocation) || !Directory.Exists(folderLocation))
                    return new AddFolderResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid folder location from config",
                    };

                if (string.IsNullOrWhiteSpace(request?.SourceFolder) || !Directory.Exists(request.SourceFolder))
                    return new AddFolderResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid source folder",
                    };

                var existing = ListFolders.List(folderLocation);

                var names = GetName(request.Name, request.SourceFolder, folderLocation, existing);


















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

        #region Private Methods

        private static (string name, string subfolder) GetName(string name, string sourceFolder, string folderLocation, FolderEntry[] existing)
        {
            string sourceFolder_leaf = Path.GetFileName(sourceFolder);

            string name_attempt = string.IsNullOrWhiteSpace(name) ? sourceFolder_leaf : name;


            // look through existing and make sure:
            //  name is unique
            //  new LiveSourceFolder will be unique (






            return ("a", "b");
        }

        #endregion
    }
}
