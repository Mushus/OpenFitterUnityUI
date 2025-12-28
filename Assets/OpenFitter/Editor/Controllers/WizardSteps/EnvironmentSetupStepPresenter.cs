using System.Collections.Generic;
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Presenter for the Environment Setup wizard step.
    /// Manages the overall environment preparation status and coordinates sub-tasks.
    /// </summary>
    public sealed class EnvironmentSetupStepPresenter : WizardStepPresenterBase
    {
        private readonly EnvironmentSetupStepView stepView;
        private readonly IOpenFitterEnvironmentService environmentService;
        private readonly EnvironmentSetupContext setupContext;
        private readonly OpenFitterSetupCoordinator setupCoordinator;

        private readonly List<SetupItemPresenter> itemPresenters = new();

        public EnvironmentSetupStepPresenter(
            OpenFitterState stateService,
            IOpenFitterEnvironmentService environmentService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            this.environmentService = environmentService;

            setupContext = environmentService.CreateEnvironmentSetupContext();
            setupCoordinator = setupContext.SetupCoordinator;

            setupCoordinator.OnStateChanged += HandleStateChanged;

            stepView = new EnvironmentSetupStepView(stepContainer);
            BindElements();
            InitializeItems();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        private void InitializeItems()
        {
            itemPresenters.Clear();
            stepView.ClearSetupItems();

            AddSetupItem(1, "Blender (ダウンロード)", SetupTarget.Blender, SetupActionType.Download, setupContext.BlenderDownloadTask, OpenFitterConstants.BlenderWebsiteUrl);
            AddSetupItem(2, "Blender (インストール)", SetupTarget.Blender, SetupActionType.Install, setupContext.BlenderInstallTask, null, I18n.Tr("インストール"), setupContext.BlenderDownloadTask);
            AddSetupItem(3, "OpenFitter Core (ダウンロード)", SetupTarget.OpenFitterCore, SetupActionType.Download, setupContext.CoreDownloadTask, OpenFitterConstants.OpenFitterCoreWebsiteUrl);
            AddSetupItem(4, "OpenFitter Core (インストール)", SetupTarget.OpenFitterCore, SetupActionType.Install, setupContext.CoreInstallTask, null, I18n.Tr("インストール"), setupContext.CoreDownloadTask);
            AddSetupItem(5, "Robust Weight Transfer (ダウンロード)", SetupTarget.RobustWeightTransfer, SetupActionType.Download, setupContext.AddonDownloadTask, OpenFitterConstants.RobustWeightTransferAddonWebsiteUrl);
            AddSetupItem(6, "Robust Weight Transfer (インストール)", SetupTarget.RobustWeightTransfer, SetupActionType.Install, setupContext.AddonInstallTask, null, I18n.Tr("インストール"), setupContext.AddonDownloadTask);
        }

        private void AddSetupItem(int index, string title, SetupTarget target, SetupActionType actionType, ISetupTask task, string? url, string? label = null, ISetupTask? prerequisite = null)
        {
            var itemView = stepView.AddSetupItem();
            var presenter = new SetupItemPresenter(
                index, title, target, actionType, task, itemView, setupCoordinator, environmentService, url, label, prerequisite);
            itemPresenters.Add(presenter);
        }

        protected override void BindElements()
        {
            stepView.OnSetupAllClicked += OnSetupAllClicked;
            stepView.OnResetClicked += OnResetEnvironmentClicked;
            stepView.SetSetupAllButtonText(I18n.Tr("セットアップ一括実行"));
        }

        protected override void UnbindElements()
        {
            stepView.OnSetupAllClicked -= OnSetupAllClicked;
            stepView.OnResetClicked -= OnResetEnvironmentClicked;
        }

        public override void Dispose()
        {
            base.Dispose();
            setupCoordinator.OnStateChanged -= HandleStateChanged;
            setupCoordinator.CancelProcessAll();
            setupCoordinator.AbortCurrentTask();
            itemPresenters.Clear();
        }

        public override bool CanProceed() => environmentService.IsEnvironmentReady();

        public override void Refresh()
        {
            foreach (var item in itemPresenters)
            {
                item.Refresh();
            }

            bool isReady = environmentService.IsEnvironmentReady();
            bool isProcessing = setupCoordinator.IsProcessingAll;

            stepView.SetSetupAllButtonEnabled(!isProcessing && !isReady);
            stepView.SetResetButtonEnabled(!isProcessing);
        }

        private void HandleStateChanged()
        {
            Refresh();
            InvokeStatusChanged();
        }

        private async void OnSetupAllClicked()
        {
            stepView.SetSetupAllButtonEnabled(false);
            stepView.SetResetButtonEnabled(false);

            await environmentService.StartSetupAllAsync(setupCoordinator);

            if (environmentService.IsEnvironmentReady())
            {
                RequestNextStep();
            }
            else
            {
                Refresh();
            }
        }

        private void OnResetEnvironmentClicked()
        {
            environmentService.ResetEnvironment(setupCoordinator);
            Refresh();
        }
    }
}
