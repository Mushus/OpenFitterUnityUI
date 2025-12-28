#nullable enable
using OpenFitter.Editor.Downloaders;
using OpenFitter.Editor.Installers;

namespace OpenFitter.Editor.Services
{
    public sealed class EnvironmentSetupContext
    {
        public EnvironmentSetupContext(
            BlenderDownloader blenderDownloadTask,
            BlenderInstaller blenderInstallTask,
            OpenFitterCoreDownloader coreDownloadTask,
            OpenFitterCoreInstaller coreInstallTask,
            BlenderAddonDownloader addonDownloadTask,
            BlenderAddonInstaller addonInstallTask,
            OpenFitterSetupCoordinator setupCoordinator)
        {
            BlenderDownloadTask = blenderDownloadTask;
            BlenderInstallTask = blenderInstallTask;
            CoreDownloadTask = coreDownloadTask;
            CoreInstallTask = coreInstallTask;
            AddonDownloadTask = addonDownloadTask;
            AddonInstallTask = addonInstallTask;
            SetupCoordinator = setupCoordinator;
        }

        public BlenderDownloader BlenderDownloadTask { get; }
        public BlenderInstaller BlenderInstallTask { get; }
        public OpenFitterCoreDownloader CoreDownloadTask { get; }
        public OpenFitterCoreInstaller CoreInstallTask { get; }
        public BlenderAddonDownloader AddonDownloadTask { get; }
        public BlenderAddonInstaller AddonInstallTask { get; }
        public OpenFitterSetupCoordinator SetupCoordinator { get; }
    }
}
