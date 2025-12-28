using System;
using System.IO;
using OpenFitter;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Downloaders;

namespace OpenFitter.Editor.Installers
{
    /// <summary>
    /// Task responsible for installing (extracting) Blender.
    /// </summary>
    public sealed class BlenderInstaller : ISetupTask
    {
        private bool isRunning;
        private SetupResult? lastResult;

        public BlenderInstaller()
        {
            this.lastResult = SetupResult.Success();
        }

        public bool IsRunning => isRunning;

        public bool IsReady => File.Exists(GetBlenderExecutablePath());

        public float Progress => IsReady ? 1f : (isRunning ? 0.5f : 0f);

        public SetupResult Start()
        {
            if (IsReady)
            {
                return SetupResult.Success();
            }

            string zipPath = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder, OpenFitterConstants.BlenderZipFileName);
            if (!File.Exists(zipPath))
            {
                return SetupResult.Failed(I18n.Tr("Blender zip not found. Please download it first."));
            }

            isRunning = true;
            try
            {
                string blenderToolsDir = Path.GetDirectoryName(zipPath);
                string finalBlenderDir = Path.Combine(blenderToolsDir, OpenFitterConstants.BlenderFolderName);

                ZipExtractionUtility.ExtractZipToDirectory(zipPath, finalBlenderDir);

                isRunning = false;
                lastResult = SetupResult.Success();
                return lastResult;
            }
            catch (Exception ex)
            {
                isRunning = false;
                lastResult = SetupResult.Failed(I18n.Tr("Blender installation failed: ") + ex.Message);
                return lastResult;
            }
        }

        public SetupResult Update()
        {
            return lastResult ?? (isRunning ? SetupResult.InProgress(I18n.Tr("Installing Blender...")) : SetupResult.Success());
        }

        public void Abort()
        {
            isRunning = false;
        }

        private string GetBlenderExecutablePath()
        {
            string blenderDir = Path.Combine(
                Directory.GetCurrentDirectory(),
                OpenFitterConstants.BlenderToolsFolder,
                OpenFitterConstants.BlenderFolderName);
            return Path.Combine(blenderDir, "blender.exe");
        }
    }
}
