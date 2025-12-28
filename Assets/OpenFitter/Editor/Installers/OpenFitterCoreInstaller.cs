using System;
using System.IO;
using OpenFitter;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Downloaders;

namespace OpenFitter.Editor.Installers
{
    /// <summary>
    /// Task responsible for installing (extracting) OpenFitter Core.
    /// </summary>
    public sealed class OpenFitterCoreInstaller : ISetupTask
    {
        private readonly OpenFitterCoreDownloader downloader;
        private bool isRunning;
        private SetupResult? lastResult;

        public OpenFitterCoreInstaller(OpenFitterCoreDownloader downloader)
        {
            this.downloader = downloader;
            this.lastResult = SetupResult.Success();
        }

        public bool IsRunning => isRunning;

        public bool IsReady => downloader != null && !string.IsNullOrEmpty(downloader.TryResolveScriptPath());

        public float Progress => IsReady ? 1f : (isRunning ? 0.5f : 0f);

        public SetupResult Start()
        {
            if (IsReady)
            {
                return SetupResult.Success();
            }

            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder, OpenFitterConstants.CoreZipFileName);
            if (!File.Exists(zipPath))
            {
                return SetupResult.Failed(I18n.Tr("OpenFitter Core zip not found. Please download it first."));
            }

            isRunning = true;
            try
            {
                string root = Path.GetDirectoryName(zipPath);
                string finalCoreDir = Path.Combine(root, OpenFitterConstants.CoreFolderName);

                ZipExtractionUtility.ExtractZipToDirectory(zipPath, finalCoreDir);

                isRunning = false;
                lastResult = SetupResult.Success();
                return lastResult;
            }
            catch (Exception ex)
            {
                isRunning = false;
                lastResult = SetupResult.Failed(I18n.Tr("OpenFitter Core installation failed: ") + ex.Message);
                return lastResult;
            }
        }

        public SetupResult Update()
        {
            return lastResult ?? (isRunning ? SetupResult.InProgress(I18n.Tr("Installing OpenFitter Core...")) : SetupResult.Success());
        }

        public void Abort()
        {
            isRunning = false;
        }
    }
}
