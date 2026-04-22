using System.Diagnostics;
using System.IO;
using LocalFolderKnowledge.ClassLib.Utils;

namespace LocalFolderKnowledge.ClassLib.Utils
{
    public static class PythonUtil
    {
        public const string VENV = "venv";

        public static void EnsureVirtualEnvironmentCreated(string baseFolder)
        {
            // Check if virtual environment already exists
            string venvPath = Path.Combine(baseFolder, VENV);
            
            if (!Directory.Exists(venvPath))
            {
                // Create virtual environment using Python
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"-m {VENV} " + venvPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                
                using (Process process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                    
                    if (process.ExitCode != 0)
                    {
                        string error = process.StandardError.ReadToEnd();
                        throw new InvalidOperationException("Failed to create virtual environment: " + error);
                    }
                }
            }
        }

        public static string GetPythonExe(string baseFolder) =>
            Path.Combine(baseFolder, VENV, FileSystemUtils.IsWindows() ? "Scripts" : "bin", "python");
    }
}