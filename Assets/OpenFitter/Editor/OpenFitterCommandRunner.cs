using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Executes OpenFitter commands by running Blender processes.
    /// </summary>
    public sealed class OpenFitterCommandRunner
    {
        public OpenFitterCommandRunner()
        {
        }

        /// <summary>
        /// Starts the Blender command asynchronously.
        /// </summary>
        /// <returns>The started Process.</returns>
        /// <exception cref="InvalidOperationException">Thrown when process fails to start.</exception>
        public Process StartCommand(string blenderPath, string arguments, LogReceivedHandler? onOutputData, LogReceivedHandler? onErrorData)
        {
            var psi = new ProcessStartInfo
            {
                FileName = blenderPath,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            // Fail Fast: Remove try-catch that swallows startup errors.
            // If Process.Start throws, it means we can't run. Arguments/Path invalid?
            // The caller should handle it.
            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) onOutputData?.Invoke(e.Data);
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data)) onErrorData?.Invoke(e.Data);
            };

            if (!process.Start())
            {
                throw new InvalidOperationException($"Failed to start process: {blenderPath}");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            return process;
        }
    }
}
