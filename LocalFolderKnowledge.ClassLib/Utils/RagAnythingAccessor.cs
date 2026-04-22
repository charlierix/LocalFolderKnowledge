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
                pythonExe = "python";
            
            // Create the process start info
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = pythonExe,
                Arguments = $"-m rag_anything.cli index --input \"{analyzefolder}\" --output \"{saveFolder}\"",
            };
            
            // Start the process and wait for completion
            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
    }
}