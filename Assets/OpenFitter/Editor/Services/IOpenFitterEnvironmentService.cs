using System.Threading.Tasks;
using OpenFitter.Editor.Downloaders;

namespace OpenFitter.Editor.Services
{
    public interface IOpenFitterEnvironmentService
    {
        string ScriptPath { get; }
        string BlenderPath { get; }

        bool IsEnvironmentReady();
        bool EnsureOpenFitterCorePath();
        bool ValidateEnvironmentForRun();

        Task StartSetupAllAsync(OpenFitterSetupCoordinator coordinator);
        void StartSetupAll(OpenFitterSetupCoordinator coordinator);

        EnvironmentValidationTask CreateEnvironmentValidationTask();

        EnvironmentSetupContext CreateEnvironmentSetupContext();

        // Task Operations
        void StartAddonDownload(OpenFitterSetupCoordinator coordinator, BlenderAddonDownloader addonDownloadTask);

        // Cleanup & Reinstall
        void RemoveBlender(OpenFitterSetupCoordinator coordinator);
        void RemoveOpenFitterCore(OpenFitterSetupCoordinator coordinator);
        void RemoveAddon(OpenFitterSetupCoordinator coordinator);
        void ResetEnvironment(OpenFitterSetupCoordinator coordinator);

        void ReinstallBlender(OpenFitterSetupCoordinator coordinator, BlenderDownloader blenderTask);
        void ReinstallOpenFitterCore(OpenFitterSetupCoordinator coordinator, OpenFitterCoreDownloader coreTask);
        void ReinstallAddon(OpenFitterSetupCoordinator coordinator, BlenderAddonDownloader addonDownloadTask);
    }
}
