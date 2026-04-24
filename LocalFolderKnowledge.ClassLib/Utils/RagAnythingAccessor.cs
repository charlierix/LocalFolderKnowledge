using System.Diagnostics;

namespace LocalFolderKnowledge.ClassLib.Utils
{
    public static class RagAnythingAccessor
    {
        public static void AnalyzeFolder(string baseFolder, string analyzefolder, string saveFolder)
        {
            // Find the Python executable
            string pythonExe = PythonUtil.GetPythonExe(baseFolder);

            if (!File.Exists(pythonExe))
                throw new ApplicationException("Couldn't find python.exe in virtual environment");


            // I don't think there is a cli, it's probably halucinated

            ProcessUtil.RunExe(
                pythonExe,
                $"-m rag_anything.cli index --input \"{analyzefolder}\" --output \"{saveFolder}\"",
                analyzefolder,
                "rag-anything had an error analyzing folder");
        }
    }
}