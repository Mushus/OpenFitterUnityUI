#nullable enable
using UnityEditor;
using UnityEngine;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;
using OpenFitter.Editor.Downloaders;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Presenter for a single setup item.
    /// Manages the state and interactions for a specific setup task.
    /// </summary>
    public sealed class SetupItemPresenter
    {
        private readonly int index;
        private readonly string title;
        private readonly SetupTarget target;
        private readonly SetupActionType actionType;
        private readonly ISetupTask primaryTask;
        private readonly ISetupTask? prerequisiteTask;
        private readonly string? websiteUrl;
        private readonly string buttonLabel;

        private readonly ISetupItemView view;
        private readonly OpenFitterSetupCoordinator coordinator;
        private readonly IOpenFitterEnvironmentService environmentService;

        public SetupItemPresenter(
            int index,
            string title,
            SetupTarget target,
            SetupActionType actionType,
            ISetupTask task,
            ISetupItemView view,
            OpenFitterSetupCoordinator coordinator,
            IOpenFitterEnvironmentService environmentService,
            string? url = null,
            string? label = null,
            ISetupTask? prerequisite = null)
        {
            this.index = index;
            this.title = title;
            this.target = target;
            this.actionType = actionType;
            this.primaryTask = task;
            this.view = view;
            this.coordinator = coordinator;
            this.environmentService = environmentService;
            this.websiteUrl = url;
            this.buttonLabel = label ?? (actionType == SetupActionType.Download ? I18n.Tr("ダウンロード") : I18n.Tr("インストール"));
            this.prerequisiteTask = prerequisite;

            InitializeView();
        }

        private void InitializeView()
        {
            view.SetTitle($"{index}. {I18n.Tr(title)}");
            view.SetActionButtonLabel(buttonLabel);

            view.OnActionClicked += HandleActionClicked;
            view.OnWebsiteClicked += HandleWebsiteClicked;
            view.OnMoreClicked += HandleMoreClicked;
        }

        public void Refresh()
        {
            bool isReady = primaryTask.IsReady;
            bool isRunning = primaryTask.IsRunning;

            string statusText = isReady ? I18n.Tr("完了") : (isRunning ? I18n.Tr("進行中") : I18n.Tr("未完了"));
            string statusClass = isReady ? "ready" : (isRunning ? "in-progress" : "not-ready");
            view.SetStatus(statusText, statusClass);

            bool canRun = !coordinator.IsProcessingAll && !isReady && !isRunning;
            if (prerequisiteTask != null && !prerequisiteTask.IsReady) canRun = false;

            view.SetActionButtonEnabled(canRun);
            view.SetMoreButtonVisible(isReady);
        }

        private void HandleActionClicked()
        {
            if (target == SetupTarget.RobustWeightTransfer && actionType == SetupActionType.Download)
            {
                environmentService.StartAddonDownload(coordinator, (BlenderAddonDownloader)primaryTask);
            }
            else
            {
                coordinator.StartTask(primaryTask);
            }
        }

        private void HandleWebsiteClicked()
        {
            if (!string.IsNullOrEmpty(websiteUrl))
            {
                Application.OpenURL(websiteUrl);
            }
        }

        private void HandleMoreClicked()
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent(I18n.Tr("削除")), false, () =>
            {
                switch (target)
                {
                    case SetupTarget.Blender: environmentService.RemoveBlender(coordinator); break;
                    case SetupTarget.OpenFitterCore: environmentService.RemoveOpenFitterCore(coordinator); break;
                    case SetupTarget.RobustWeightTransfer: environmentService.RemoveAddon(coordinator); break;
                }
            });

            menu.AddItem(new GUIContent(I18n.Tr("再取得")), false, () =>
            {
                switch (target)
                {
                    case SetupTarget.Blender:
                        environmentService.ReinstallBlender(coordinator, (BlenderDownloader)primaryTask);
                        break;
                    case SetupTarget.OpenFitterCore:
                        environmentService.ReinstallOpenFitterCore(coordinator, (OpenFitterCoreDownloader)primaryTask);
                        break;
                    case SetupTarget.RobustWeightTransfer:
                        environmentService.ReinstallAddon(coordinator, (BlenderAddonDownloader)primaryTask);
                        break;
                }
            });
            menu.ShowAsContext();
        }
    }
}
