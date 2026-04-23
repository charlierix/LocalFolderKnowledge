using System.Diagnostics;
using System.IO;
using LocalFolderKnowledge.ClassLib.Utils;

namespace LocalFolderKnowledge.ClassLib.Utils
{
    public static class PythonUtil
    {
        public const string VIRTUAL_ENV = ".venv";

        public static string GetPythonExe(string baseFolder) =>
            Path.Combine(baseFolder, VIRTUAL_ENV, FileSystemUtils.IsWindows() ? "Scripts" : "bin", "python");

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
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = $"-m venv {VIRTUAL_ENV}",

                UseShellExecute = false,
                WorkingDirectory = folder,

                WindowStyle = ProcessWindowStyle.Hidden,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
            };

            using (Process process = Process.Start(startInfo))
            {
                process?.WaitForExit();
                ThrowIfError(process, "Error setting up virtual environment");
            }

            PipInstall(folder, "raganything", true);
        }

        // This will do a pip install, virtual environment must be set before this
        private static void PipInstall(string folder, string packageName, bool isUpgrade)
        {
            string pythonExePath = GetPythonExe(folder);

            string args = "-m pip install";

            if (isUpgrade)
                args += " --upgrade";

            args += $" {packageName}";

            var startInfo = new ProcessStartInfo
            {
                FileName = pythonExePath,
                Arguments = args,

                UseShellExecute = false,
                WorkingDirectory = folder,

                WindowStyle = ProcessWindowStyle.Hidden,

                RedirectStandardOutput = false,
                RedirectStandardError = true,
            };

            using (Process? process = Process.Start(startInfo))
            {
                process?.WaitForExit();
                ThrowIfError(process, "Error running pip install");
            }
        }

        private static void ThrowIfError(Process? process, string message)
        {
            if (process == null)
                throw new ApplicationException($"{message}: process is null (exe probably doesn't exist or not in path)");

            if (process.ExitCode != 0)
            {
                string error = process.StartInfo.RedirectStandardError ?
                    process.StandardError.ReadToEnd() :
                    "UNKNOWN ERROR, StandardError wasn't redirected";
                throw new InvalidOperationException($"{message}: {error}");
            }

            if (!process.StartInfo.RedirectStandardError)
                return;

            string exception = process.StandardError.ReadToEnd();
            if (string.IsNullOrEmpty(exception))
                return;

            // throw out lines that start with "[notice] "
            string[] lines = exception.
                Replace("\r\n", "\n").
                Replace("\r", "\n").
                Split('\n', StringSplitOptions.RemoveEmptyEntries).
                Select(o => o.Trim()).
                Where(o => !o.StartsWith("[notice] ", StringComparison.OrdinalIgnoreCase)).
                ToArray();

            if (lines.Length == 0)
                return;

            exception = string.Join('\n', lines);

            throw new ApplicationException($"{message}:{Environment.NewLine}{process.StartInfo.FileName} {process.StartInfo.ArgumentList}{Environment.NewLine}{exception}");
        }
    }
}