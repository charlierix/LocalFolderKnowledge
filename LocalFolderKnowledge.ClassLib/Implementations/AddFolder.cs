using LocalFolderKnowledge.ClassLib.Models;
using LocalFolderKnowledge.ClassLib.Utils;
using System.Text.Json;

namespace LocalFolderKnowledge.ClassLib.Implementations
{
    public static class AddFolder
    {
        public static AddFolderResponse Add(AddFolderRequest request, string folderLocation)
        {
            try
            {
                // Validate configuration path
                if (string.IsNullOrWhiteSpace(folderLocation) || !Directory.Exists(folderLocation))
                {
                    return new AddFolderResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid folder location from config",
                    };
                }

                // Validate source folder
                if (string.IsNullOrWhiteSpace(request?.SourceFolder) || !Directory.Exists(request.SourceFolder))
                {
                    return new AddFolderResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = "Invalid source folder",
                    };
                }

                // Get existing folders to help with ensuring name uniqueness
                var existing = ListFolders.List(folderLocation);

                // Figure out unique name and folder name
                var names = GetName(request.Name, request.SourceFolder, folderLocation, existing);

                // Create subfolder under folderLocation (D:\LocalFolderKnowledge), copy request folder if told to
                var entry = CreateSubfolder(request, names.name, folderLocation, names.subfolder);



                // use a pool of long running threads to parse this folder

                RagAnythingAccessor.AnalyzeFolder(entry.source_folder, entry.rag_folder);




                return new AddFolderResponse
                {
                    IsSuccess = true,
                    Entry = entry.entry,
                };
            }
            catch (Exception ex)
            {
                return new AddFolderResponse
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        #region Private Methods

        private static (string name, string subfolder) GetName(string name, string sourceFolder, string folderLocation, FolderEntry[] existing)
        {
            string sourceFolder_leaf = Path.GetFileName(sourceFolder);

            string name_attempt = string.IsNullOrWhiteSpace(name) ?
                sourceFolder_leaf :
                name;

            string escaped_name = FileSystemUtils.EscapeFilename(name_attempt);

            // Make sure name is unique
            string new_name = name_attempt;
            string new_subfolder = FileSystemUtils.EscapeFilename(name_attempt);

            int counter = 0;

            while (true)
            {
                // Check for either to make sure they get the same unique number suffix
                if (existing.FirstOrDefault(o => o.Name.Equals(new_name, StringComparison.OrdinalIgnoreCase) || o.SubFolder.Equals(new_subfolder, StringComparison.OrdinalIgnoreCase)) == null)
                    break;

                counter++;
                new_name = $"{name_attempt} {counter}";
                new_subfolder = $"{escaped_name} {counter}";
            }

            return (new_name, new_subfolder);
        }

        private static (FolderEntry entry, string source_folder, string rag_folder) CreateSubfolder(AddFolderRequest request, string name, string folderLocation, string subfolder)
        {
            var entry = new FolderEntry
            {
                Name = name,
                OriginalSourceFolder = request.SourceFolder,
                SubFolder = subfolder,
                IsCopy = request.ShouldCopyContents,
                IsFinishedParsing = false,
            };

            var folders = entry.GetFolders(folderLocation);

            Directory.CreateDirectory(folders.sub_folder);

            if (request.ShouldCopyContents)
                CopyDirectory(request.SourceFolder, folders.source_folder, true);

            var entrySettings = new EntrySettings
            {
                Entry = entry,
            };

            string jsonString = JsonSerializer.Serialize(entrySettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(folders.sub_folder, EntrySettings.SETTINGS_FILENAME), jsonString);

            return (entry, folders.sub_folder, folders.rag_folder);
        }

        private static void CopyDirectory(string sourceDir, string destinationDir, bool recursive = true)
        {
            var dir = new DirectoryInfo(sourceDir);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source: {dir.FullName}");

            Directory.CreateDirectory(destinationDir);

            // Copy files
            foreach (FileInfo file in dir.GetFiles())
                file.CopyTo(Path.Combine(destinationDir, file.Name), true);

            // Recursive copy subdirectories
            if (recursive)
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                    CopyDirectory(subDir.FullName, Path.Combine(destinationDir, subDir.Name), true);
        }

        #endregion
    }
}