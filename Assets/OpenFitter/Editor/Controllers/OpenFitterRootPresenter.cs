using System;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Downloaders;
using OpenFitter.Editor.Installers;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor.Controllers
{
    public sealed class OpenFitterRootPresenter : IDisposable
    {
        private readonly OpenFitterState stateService;
        private readonly OpenFitterEnvironmentService environmentService;
        private readonly ConfigurationService configurationService;
        private readonly OpenFitterWizardPresenter wizardPresenter;
        public OpenFitterRootPresenter(OpenFitterRootView rootView)
        {
            stateService = new OpenFitterState();

            var coreDownloader = new OpenFitterCoreDownloader();
            var addonDownloader = new BlenderAddonDownloader(
                OpenFitterConstants.RobustWeightTransferAddonUrl,
                OpenFitterConstants.RobustWeightTransferAddonName,
                OpenFitterConstants.RobustWeightTransferAddonZipFileName);

            environmentService = new OpenFitterEnvironmentService(
                coreDownloader,
                new BlenderAddonInstaller(OpenFitterConstants.RobustWeightTransferAddonName, addonDownloader),
                stateService
            );

            configurationService = new ConfigurationService();
            configurationService.RefreshAvailableConfigs();

            // Get the wizard host from the root view
            var wizardHost = rootView.GetWizardHost();

            var wizardView = new OpenFitterWizardView(wizardHost);
            wizardPresenter = new OpenFitterWizardPresenter(stateService, environmentService, configurationService, wizardView);
        }

        public void OnEnable()
        {
            I18n.Reload();
            environmentService.EnsureOpenFitterCorePath();
            configurationService.RefreshAvailableConfigs();
        }

        public void OnDisable()
        {
            wizardPresenter.Dispose();
        }

        public void Dispose() => OnDisable();

        public void RefreshProjectJson()
        {
            configurationService.RefreshAvailableConfigs();
            configurationService.RefreshJsonAssets();
        }
    }
}
