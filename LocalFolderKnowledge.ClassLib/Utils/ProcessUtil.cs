using System.Diagnostics;

namespace LocalFolderKnowledge.ClassLib.Utils
{
    public static class ProcessUtil
    {
        public static void RunExe(string filename, string arguments, string workingDir, string errorMsg)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = filename,
                Arguments = arguments,

                UseShellExecute = false,
                WorkingDirectory = workingDir,

                //WindowStyle = ProcessWindowStyle.Hidden,
                //RedirectStandardOutput = true,

                WindowStyle = ProcessWindowStyle.Normal,
                RedirectStandardOutput = false,

                RedirectStandardError = true,
            };

            using (Process? process = Process.Start(startInfo))
            {
                process?.WaitForExit();
                ProcessUtil.ThrowIfError(process, errorMsg);
            }
        }

        public static void ThrowIfError(Process? process, string message)
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
