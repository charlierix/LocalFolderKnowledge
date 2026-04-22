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
        public string SubFolder { get; init; }

        public bool IsCopy { get; init; }

        // When a folder is first added, it takes a while to parse it.  This will be true when that is finished and
        // it's ready to be used
        public bool IsFinishedParsing { get; init; }

        /// <summary>
        /// Helper function that returns full path to various folders
        /// </summary>
        /// <param name="folderLocation">what's in appsettings.FolderLocation</param>
        /// <returns>
        /// sub_folder: folderLocation + LiveSourceFolder
        /// source_folder: either OriginalSourceFolder or the copy
        /// rag_folder: sub_folder + RAG_FOLDER
        /// </returns>
        public (string sub_folder, string source_folder, string rag_folder) GetFolders(string folderLocation)
        {
            string sub_folder = Path.Combine(folderLocation, SubFolder);

            string source_folder = IsCopy ?
                Path.Combine(sub_folder, COPY_FOLDER) :
                OriginalSourceFolder;

            string rag_folder = Path.Combine(sub_folder, RAG_FOLDER);

            return (sub_folder, source_folder, rag_folder);
        }
    }
}
