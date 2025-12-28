using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using OpenFitter.Editor.Downloaders;
using OpenFitter.Editor.Installers;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterEnvironmentService : IOpenFitterEnvironmentService
    {
        private readonly OpenFitterCoreDownloader coreDownloader;
        private readonly BlenderAddonInstaller addonInstaller;
        private readonly OpenFitterState stateService;
        private readonly EnvironmentValidationTask environmentValidationTask;

        public OpenFitterEnvironmentService(
            OpenFitterCoreDownloader coreDownloader,
            BlenderAddonInstaller addonInstaller,
            OpenFitterState stateService)
        {
            this.coreDownloader = coreDownloader;
            this.addonInstaller = addonInstaller;
            this.stateService = stateService;
            environmentValidationTask = new EnvironmentValidationTask(addonInstaller, BlenderPath, coreDownloader);
        }

        public string ScriptPath => coreDownloader.TryResolveScriptPath();

        public string BlenderPath
        {
            get
            {
                string blenderDir = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    OpenFitterConstants.BlenderToolsFolder,
                    OpenFitterConstants.BlenderFolderName);
                return Path.Combine(blenderDir, "blender.exe");
            }
        }

        public bool EnsureOpenFitterCorePath()
        {
            return !string.IsNullOrEmpty(ScriptPath);
        }

        public bool IsBlenderReady()
        {
            return environmentValidationTask.IsBlenderReady();
        }

        public bool IsAddonReady()
        {
            return environmentValidationTask.IsAddonReady();
        }

        public bool IsOpenFitterCoreReady()
        {
            return environmentValidationTask.IsOpenFitterCoreReady();
        }

        public bool IsEnvironmentReady()
        {
            return environmentValidationTask.IsReady;
        }

        public bool ValidateEnvironmentForRun()
        {
            EnsureOpenFitterCorePath();

            SetupResult result = environmentValidationTask.Start();

            if (!result.IsSuccess)
            {
                EditorUtility.DisplayDialog(
                    I18n.Tr("Environment Setup"),
                    result.Message,
                    I18n.Tr("OK"));
                return false;
            }

            return true;
        }

        public EnvironmentValidationTask CreateEnvironmentValidationTask()
        {
            return new EnvironmentValidationTask(addonInstaller, BlenderPath, coreDownloader);
        }

        public EnvironmentSetupContext CreateEnvironmentSetupContext()
        {
            var blenderDownloadTask = new BlenderDownloader();
            var blenderInstallTask = new BlenderInstaller();
            var coreDownloadTask = coreDownloader;
            var coreInstallTask = new OpenFitterCoreInstaller(coreDownloader);
            var addonDownloadTask = new BlenderAddonDownloader(
                OpenFitterConstants.RobustWeightTransferAddonUrl,
                OpenFitterConstants.RobustWeightTransferAddonName,
                OpenFitterConstants.RobustWeightTransferAddonZipFileName
            );
            var addonInstallTask = addonInstaller;

            var setupCoordinator = CreateSetupCoordinator(
                blenderDownloadTask,
                blenderInstallTask,
                coreDownloadTask,
                coreInstallTask,
                addonDownloadTask,
                addonInstallTask);

            return new EnvironmentSetupContext(
                blenderDownloadTask,
                blenderInstallTask,
                coreDownloadTask,
                coreInstallTask,
                addonDownloadTask,
                addonInstallTask,
                setupCoordinator);
        }

        private OpenFitterSetupCoordinator CreateSetupCoordinator(
            BlenderDownloader blenderDownloadTask,
            BlenderInstaller blenderInstallTask,
            OpenFitterCoreDownloader coreDownloadTask,
            OpenFitterCoreInstaller coreInstallTask,
            BlenderAddonDownloader addonDownloadTask,
            BlenderAddonInstaller addonInstallTask)
        {
            var entries = new List<OpenFitterSetupCoordinator.SetupEntry>
            {
                // 1. Blender
                new(
                I18n.Tr("Blender Download"),
                blenderDownloadTask,
                websiteUrl: OpenFitterConstants.BlenderWebsiteUrl
            ),
                new(
                I18n.Tr("Blender Install"),
                blenderInstallTask
            ),

                // 2. Core
                new(
                I18n.Tr("OpenFitter Core Download"),
                coreDownloadTask,
                websiteUrl: OpenFitterConstants.OpenFitterCoreWebsiteUrl
            ),
                new(
                I18n.Tr("OpenFitter Core Install"),
                coreInstallTask
            ), 

                // 3. Addon
                new(
                I18n.Tr("Blender Addon Download"),
                addonDownloadTask,
                websiteUrl: OpenFitterConstants.RobustWeightTransferAddonWebsiteUrl
            ),
                new(
                I18n.Tr("Blender Addon Install"),
                addonInstallTask
            )
            };

            return new OpenFitterSetupCoordinator(entries);
        }

        // Validation Methods
        public bool ValidateAddonDownloadPreconditions(out string errorMessage)
        {
            if (string.IsNullOrEmpty(BlenderPath) ||
                !File.Exists(BlenderPath))
            {
                errorMessage = I18n.Tr("Blender path is not set or invalid.");
                return false;
            }
            errorMessage = string.Empty;
            return true;
        }

        // Cleanup Operations
        private void PerformCleanup(
            OpenFitterSetupCoordinator coordinator,
            Action deleteFilesAction,
            Action? resetStateAction)
        {
            coordinator.CancelProcessAll();
            coordinator.AbortCurrentTask();
            deleteFilesAction?.Invoke();
            resetStateAction?.Invoke();
            AssetDatabase.Refresh();
        }

        public void RemoveBlender(OpenFitterSetupCoordinator coordinator)
        {
            PerformCleanup(
                coordinator,
                OpenFitterFileUtility.DeleteBlenderFiles,
                null);
        }

        public void RemoveOpenFitterCore(OpenFitterSetupCoordinator coordinator)
        {
            PerformCleanup(
                coordinator,
                OpenFitterFileUtility.DeleteCoreFiles,
                null);
        }

        public void RemoveAddon(OpenFitterSetupCoordinator coordinator)
        {
            PerformCleanup(
                coordinator,
                OpenFitterFileUtility.DeleteAddonFiles,
                null);
        }

        public void ResetEnvironment(OpenFitterSetupCoordinator coordinator)
        {
            PerformCleanup(
                coordinator,
                OpenFitterFileUtility.ClearCacheFiles,
                () =>
                {
                    stateService.ResetEnvironmentState();
                    stateService.BlendShapeEntries.Clear();
                });
        }

        // Reinstall Operations
        public void ReinstallBlender(
            OpenFitterSetupCoordinator coordinator,
            BlenderDownloader blenderTask)
        {
            RemoveBlender(coordinator);
            coordinator.StartTask(blenderTask);
        }

        public void ReinstallOpenFitterCore(
            OpenFitterSetupCoordinator coordinator,
            OpenFitterCoreDownloader coreTask)
        {
            RemoveOpenFitterCore(coordinator);
            coordinator.StartTask(coreTask);
        }

        public void ReinstallAddon(
            OpenFitterSetupCoordinator coordinator,
            BlenderAddonDownloader addonDownloadTask)
        {
            RemoveAddon(coordinator);
            StartAddonDownload(coordinator, addonDownloadTask);
        }

        // Task Start Methods
        public void StartAddonDownload(
            OpenFitterSetupCoordinator coordinator,
            BlenderAddonDownloader addonDownloadTask)
        {
            if (!ValidateAddonDownloadPreconditions(out string errorMessage))
            {
                SetupResultHandler.HandleError(
                    SetupResult.Failed(errorMessage),
                    I18n.Tr("Blender Addon Download"));
                return;
            }
            coordinator.StartTask(addonDownloadTask);
        }

        public Task StartSetupAllAsync(OpenFitterSetupCoordinator coordinator)
        {
            return coordinator.ProcessAllAsync();
        }

        public void StartSetupAll(OpenFitterSetupCoordinator coordinator)
        {
            coordinator.ProcessAll();
        }
    }
}
