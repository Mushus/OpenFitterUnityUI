#nullable enable
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class ExecutionStepPresenter : WizardStepPresenterBase
    {
        private readonly ExecutionStepView stepView;
        private readonly FittingService fittingService;
        private readonly IOpenFitterEnvironmentService environmentService;
        private readonly ConfigurationService configService;

        public ExecutionStepPresenter(
            OpenFitterState stateService,
            IOpenFitterEnvironmentService environmentService,
            ConfigurationService configService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            this.environmentService = environmentService;
            this.configService = configService;

            fittingService = new FittingService(environmentService);

            stepView = new ExecutionStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            fittingService.OnLogReceived += OnLogReceived;
            fittingService.OnProgressChanged += OnProgressChanged;
            fittingService.OnStatusChanged += OnStatusChanged;
            fittingService.OnStepChanged += OnStepChanged;
        }

        protected override void UnbindElements()
        {
            fittingService.OnLogReceived -= OnLogReceived;
            fittingService.OnProgressChanged -= OnProgressChanged;
            fittingService.OnStatusChanged -= OnStatusChanged;
            fittingService.OnStepChanged -= OnStepChanged;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (CanExecuteFitting() && !fittingService.IsFitting)
            {
                ExecuteFitting();
            }

            Refresh();
        }

        public override bool CanProceed()
        {
            return !fittingService.IsFitting;
        }

        public override void Refresh()
        {
            stepView.SetStatus(string.Format(I18n.Tr("Status: {0}"), fittingService.LastRunSummary));
            UpdateStatusBadge();
        }

        private void OnLogReceived(string log)
        {
            stepView.AppendLog(log);
            stepView.ScrollLogToBottom();
        }

        private void OnProgressChanged(float progress, string statusDetail)
        {
            if (progress > 0)
            {
                stepView.SetProgress(progress * 100f, $"{(int)(progress * 100f)}%");
            }

            if (!string.IsNullOrEmpty(statusDetail))
            {
                stepView.SetStatus($"Status: {statusDetail}");
            }
        }

        private void OnStatusChanged(string status)
        {
            UpdateStatusBadge();
            InvokeStatusChanged();

            if (status == "Completed")
            {
                stepView.SetProgress(100, "100%");
                stateService.LastOutputPath = fittingService.LastOutputPath;
            }
        }

        private void OnStepChanged(int current, int total)
        {
            // Future: Update UI with step info
        }

        private void UpdateStatusBadge()
        {
            var state = FittingProgressParser.AnalyzeExecutionStateStatic(fittingService.IsFitting, fittingService.LastRunSummary);

            switch (state)
            {
                case FittingExecutionState.Processing:
                    stepView.SetStatusBadge(I18n.Tr("Processing..."), "processing");
                    break;
                case FittingExecutionState.Completed:
                    stepView.SetStatusBadge(I18n.Tr("Completed"), "completed");
                    break;
                case FittingExecutionState.Error:
                    stepView.SetStatusBadge(I18n.Tr("Error"), "failed");
                    break;
                case FittingExecutionState.Idle:
                default:
                    stepView.SetStatusBadge(I18n.Tr("Idle"), "");
                    break;
            }
        }

        public override void Dispose()
        {
            if (fittingService.IsFitting)
            {
                fittingService.CancelFitting();
            }
            fittingService.Dispose();
            base.Dispose();
        }

        private bool CanExecuteFitting()
        {
            return environmentService.IsEnvironmentReady()
                   && !string.IsNullOrEmpty(stateService.SourceConfigPath)
                   && !string.IsNullOrEmpty(stateService.TargetConfigPath);
        }

        private void ExecuteFitting()
        {
            if (!environmentService.ValidateEnvironmentForRun())
            {
                return;
            }

            fittingService.ExecuteFitting(stateService, stateService.BlendShapeEntries, configService.AvailableConfigs);

            stateService.LastOutputPath = fittingService.LastOutputPath;
        }
    }
}
