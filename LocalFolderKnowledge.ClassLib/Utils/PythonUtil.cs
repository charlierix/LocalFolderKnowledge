using System.Diagnostics;
using System.IO;
using LocalFolderKnowledge.ClassLib.Utils;

namespace LocalFolderKnowledge.ClassLib.Utils
{
    public static class PythonUtil
    {
        public const string VIRTUAL_ENV = ".venv";

        public static string GetPythonExe(string baseFolder) =>
            Path.Combine(baseFolder, VIRTUAL_ENV, FileSystemUtils.IsWindows() ? "Scripts" : "bin", "python.exe");

        /// <summary>
        /// Makes sure virtual environment is created, does pip installs
        /// </summary>
        public static void EnsureVirtualEnvironmentCreated(string folder)
        {
            if (!Directory.Exists(folder))
                throw new ArgumentException($"folder passed in doesn't exist: {folder}");

            // Check if virtual environment already exists
            if (Directory.Exists(Path.Combine(folder, VIRTUAL_ENV)))
                return;

            // Create virtual environment using Python
            // NOTE: raganything pip install fails with 3.14 (as of 5/1/2026)
            ProcessUtil.RunExe("py", $"-3.13 -m venv {VIRTUAL_ENV}", folder, "Error setting up virtual environment");

            UpgradePython(folder);

            PipInstall(folder, "raganything[all]", true);
            PipInstall(folder, "ollama", true);
        }

        private static void UpgradePython(string folder)
        {
            //python.exe -m pip install --upgrade pip
            string pythonExePath = GetPythonExe(folder);
            string args = "-m pip install --upgrade pip";

            ProcessUtil.RunExe(pythonExePath, args, folder, "Error upgrading pip");
        }

        // This will do a pip install, virtual environment must be set before this
        private static void PipInstall(string folder, string packageName, bool isUpgrade)
        {
            string pythonExePath = GetPythonExe(folder);

            string args = "-m pip install";

            if (isUpgrade)
                args += " --upgrade";

            args += $" {packageName}";

            ProcessUtil.RunExe(pythonExePath, args, folder, "Error running pip install");
        }
    }
}