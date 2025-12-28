using System;
using System.IO;
using OpenFitter;
using UnityEditor;
using UnityEngine;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Downloaders;

namespace OpenFitter.Editor.Installers
{
    /// <summary>
    /// Handles installation of downloaded Blender addons to the appropriate Blender addons directory.
    /// Responsibility: Extraction and installation.
    /// </summary>
    public sealed class BlenderAddonInstaller : ISetupTask
    {
        private readonly string addonName;
        private readonly Downloaders.BlenderAddonDownloader downloader;
        private bool isRunning;
        private SetupResult? lastResult;

        public BlenderAddonInstaller(string addonName, Downloaders.BlenderAddonDownloader downloader)
        {
            this.addonName = addonName ?? throw new ArgumentNullException(nameof(addonName));
            this.downloader = downloader;
            lastResult = SetupResult.Success();
        }

        public bool IsRunning => isRunning;

        public bool IsReady => IsAddonInstalled();

        public float Progress => IsReady ? 1f : (isRunning ? 0.5f : 0f);

        public SetupResult Start()
        {
            if (IsReady) return SetupResult.Success();

            if (!File.Exists(downloader.ZipPath))
            {
                return SetupResult.Failed(I18n.Tr("Addon zip not found. Please download it first."));
            }

            isRunning = true;
            try
            {
                string blenderPath = GetBlenderPath();
                if (string.IsNullOrEmpty(blenderPath) || !File.Exists(blenderPath))
                {
                    isRunning = false;
                    return SetupResult.Failed(I18n.Tr("Blender path is not set or invalid. Please setup Blender first."));
                }

                // 1. Extract to temp/local location first
                string tempExtractPath = Path.Combine(Path.GetTempPath(), "OpenFitter", "addons", "extracted_" + Guid.NewGuid());
                ZipExtractionUtility.ExtractZipToDirectory(downloader.ZipPath, tempExtractPath);

                // 2. Install from extracted path
                InstallAddon(blenderPath, tempExtractPath);

                // Cleanup temp
                if (Directory.Exists(tempExtractPath)) Directory.Delete(tempExtractPath, true);

                isRunning = false;
                lastResult = SetupResult.Success();
                return lastResult;
            }
            catch (Exception ex)
            {
                isRunning = false;
                lastResult = SetupResult.Failed(I18n.Tr("Addon installation failed: ") + ex.Message);
                return lastResult;
            }
        }

        public SetupResult Update()
        {
            return lastResult ?? (isRunning ? SetupResult.InProgress(I18n.Tr("Installing Addon...")) : SetupResult.Success());
        }

        public void Abort()
        {
            isRunning = false;
        }

        private string GetBlenderPath()
        {
            string blenderDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                OpenFitterConstants.BlenderToolsFolder,
                OpenFitterConstants.BlenderFolderName);
            return Path.Combine(blenderDir, "blender.exe");
        }

        /// <summary>
        /// Installs an extracted addon to the specified Blender installation.
        /// </summary>
        /// <param name="blenderPath">Path to the Blender executable</param>
        /// <param name="extractedAddonPath">Path to the extracted addon directory</param>
        public void InstallAddon(string blenderPath, string extractedAddonPath)
        {
            if (string.IsNullOrEmpty(blenderPath))
            {
                throw new ArgumentException(I18n.Tr("Blender path is not set."));
            }

            if (!File.Exists(blenderPath))
            {
                throw new FileNotFoundException($"Blender executable not found: {blenderPath}");
            }

            if (string.IsNullOrEmpty(extractedAddonPath) || !Directory.Exists(extractedAddonPath))
            {
                throw new DirectoryNotFoundException($"Extracted addon not found: {extractedAddonPath}");
            }

            // 1. First, install to a persistent local directory (Copy files)
            string addonsDirectory = GetAddonsDirectory();

            if (!Directory.Exists(addonsDirectory))
            {
                Directory.CreateDirectory(addonsDirectory);
            }

            string targetAddonPath = Path.Combine(addonsDirectory, addonName);

            // Remove existing addon if present
            if (Directory.Exists(targetAddonPath))
            {
                Directory.Delete(targetAddonPath, true);
            }

            // Copy the extracted addon to the local addons directory
            Debug.Log($"[OpenFitter] Copying addon files to: {targetAddonPath}");
            CopyDirectory(extractedAddonPath, targetAddonPath);

            // 2. Now run the Blender install script to setup dependencies (like scipy)
            // This MUST happen after files are in place so Blender can find the addon.
            Debug.Log("[OpenFitter] Installing addon dependencies via Blender...");
            RunBlenderInstallScript(blenderPath);

            Debug.Log($"[OpenFitter] Addon '{addonName}' installation process completed.");
            AssetDatabase.Refresh();
        }

        public bool IsAddonInstalled()
        {
            string addonsDirectory = GetAddonsDirectory();

            if (!Directory.Exists(addonsDirectory))
            {
                return false;
            }

            string addonPath = Path.Combine(addonsDirectory, addonName);
            string markerFile = Path.Combine(addonPath, "install_complete.marker");

            if (File.Exists(markerFile))
            {
                return true;
            }

            return false;
        }

        public void RunStatusCheck(string blenderPath)
        {
            if (string.IsNullOrEmpty(blenderPath) || !File.Exists(blenderPath)) return;

            string checkScriptPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Assets", "OpenFitter", "Editor", "Resources",
                "check_addon_status.py"
            );

            if (!File.Exists(checkScriptPath)) return;

            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = blenderPath,
                Arguments = $"--background --python \"{checkScriptPath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            try
            {
                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    process?.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[OpenFitter] Status check failed: {ex.Message}");
            }
        }

        private void CopyDirectory(string sourceDir, string targetDir)
        {
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFile = Path.Combine(targetDir, Path.GetFileName(file));
                File.Copy(file, targetFile, overwrite: true);
            }

            foreach (string dir in Directory.GetDirectories(sourceDir))
            {
                string targetSubDir = Path.Combine(targetDir, Path.GetFileName(dir));
                CopyDirectory(dir, targetSubDir);
            }
        }

        private static string GetAddonsDirectory()
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            return Path.Combine(root, "addons");
        }

        private void RunBlenderInstallScript(string blenderPath)
        {
            try
            {
                string installScriptPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "Assets", "OpenFitter", "Editor", "Resources",
                    "install_addon_dependencies.py"
                );

                if (!File.Exists(installScriptPath))
                {
                    Debug.LogError($"[OpenFitter] Install script not found: {installScriptPath}");
                    return;
                }

                var psi = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = blenderPath,
                    Arguments = $"--background --python \"{installScriptPath}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                Debug.Log($"[OpenFitter] Running install script: {blenderPath} {psi.Arguments}");

                using (var process = System.Diagnostics.Process.Start(psi))
                {
                    if (process == null)
                    {
                        Debug.LogError("[OpenFitter] Failed to start Blender for dependency installation");
                        return;
                    }

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        Debug.Log($"[OpenFitter] Install output:\n{output}");
                    }

                    if (process.ExitCode == 0)
                    {
                        Debug.Log("[OpenFitter] Addon dependencies setup triggered. Performing final verification...");
                        // Run immediate status check to verify result and create marker
                        RunStatusCheck(blenderPath);
                    }
                    else
                    {
                        Debug.LogWarning($"[OpenFitter] Addon dependency installation completed with exit code {process.ExitCode}");
                        if (!string.IsNullOrEmpty(error))
                        {
                            Debug.LogError($"[OpenFitter] Install error:\n{error}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[OpenFitter] Error running install script: {ex.Message}");
            }
        }
    }
}
