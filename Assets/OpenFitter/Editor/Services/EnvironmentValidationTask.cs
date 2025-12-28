using System;
using System.IO;
using OpenFitter;
using OpenFitter.Editor.Installers;
using UnityEditor;

namespace OpenFitter.Editor.Services
{
    /// <summary>
    /// Task responsible for validating the OpenFitter environment setup.
    /// </summary>
    public class EnvironmentValidationTask : ISetupTask
    {
        private readonly BlenderAddonInstaller addonInstaller;
        private readonly string blenderPath;
        private readonly OpenFitter.Editor.Downloaders.OpenFitterCoreDownloader coreDownloader;
        private bool isRunning;
        private SetupResult? lastResult;

        public EnvironmentValidationTask(
            BlenderAddonInstaller addonInstaller,
            string blenderPath,
            OpenFitter.Editor.Downloaders.OpenFitterCoreDownloader coreDownloader)
        {
            this.addonInstaller = addonInstaller ?? throw new ArgumentNullException(nameof(addonInstaller));
            this.blenderPath = blenderPath ?? throw new ArgumentNullException(nameof(blenderPath));
            this.coreDownloader = coreDownloader ?? throw new ArgumentNullException(nameof(coreDownloader));
        }

        public bool IsRunning => isRunning;

        public bool IsReady => IsBlenderReady() && IsAddonReady() && IsOpenFitterCoreReady();

        public float Progress => IsReady ? 1f : (isRunning ? 0.5f : 0f);

        public SetupResult Start()
        {
            if (IsReady)
            {
                lastResult = SetupResult.Success();
                return lastResult;
            }

            isRunning = true;
            try
            {
                lastResult = ValidateEnvironment();
                isRunning = false;
            }
            catch (Exception ex)
            {
                isRunning = false;
                lastResult = SetupResult.Failed(ex.Message);
            }

            return lastResult;
        }

        public SetupResult Update()
        {
            return lastResult ?? SetupResult.Success();
        }

        public void Abort()
        {
            isRunning = false;
        }

        public bool IsBlenderReady()
        {
            return File.Exists(blenderPath);
        }

        public bool IsAddonReady()
        {
            return addonInstaller.IsAddonInstalled();
        }

        public bool IsOpenFitterCoreReady()
        {
            return File.Exists(coreDownloader.TryResolveScriptPath());
        }

        private SetupResult ValidateEnvironment()
        {
            if (!IsBlenderReady())
            {
                return SetupResult.Failed(I18n.Tr("Blender is not ready. Please download Blender first."));
            }

            if (!IsAddonReady())
            {
                return SetupResult.Failed(I18n.Tr("Blender addon is not ready. Please download the addon first."));
            }

            if (!IsOpenFitterCoreReady())
            {
                return SetupResult.Failed(I18n.Tr("OpenFitter Core is not ready. Please download OpenFitter Core first."));
            }

            return SetupResult.Success();
        }
    }
}
