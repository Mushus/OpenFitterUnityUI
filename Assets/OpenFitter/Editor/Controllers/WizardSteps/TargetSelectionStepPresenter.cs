#nullable enable
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class TargetSelectionStepPresenter : WizardStepPresenterBase
    {
        private readonly TargetSelectionStepView stepView;
        private readonly ConfigurationService configService;

        public TargetSelectionStepPresenter(
            OpenFitterState stateService,
            ConfigurationService configService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            this.configService = configService;
            stepView = new TargetSelectionStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            stepView.OnTargetIndexChanged += OnTargetIndexChanged;
            stepView.OnSourceIndexChanged += OnSourceIndexChanged;
        }

        protected override void UnbindElements()
        {
            stepView.OnTargetIndexChanged -= OnTargetIndexChanged;
            stepView.OnSourceIndexChanged -= OnSourceIndexChanged;
        }

        private void OnTargetIndexChanged(int index)
        {
            if (index > 0 && index - 1 < configService.CurrentTargetConfigs.Count)
            {
                var config = configService.CurrentTargetConfigs[index - 1];
                stateService.TargetConfigPath = config.configPath;
            }
            else
            {
                stateService.TargetConfigPath = "";
            }
            InvokeStatusChanged();
        }

        private void OnSourceIndexChanged(int index)
        {
            if (index > 0 && index - 1 < configService.CurrentSourceConfigs.Count)
            {
                var config = configService.CurrentSourceConfigs[index - 1];
                stateService.SourceConfigPath = config.configPath;

                if (config != null)
                {
                    var entries = configService.LoadBlendShapes(config);
                    stateService.SetBlendShapeEntries(entries);
                }
            }
            else
            {
                stateService.SourceConfigPath = "";
                stateService.SetBlendShapeEntries(null!);
            }
            InvokeStatusChanged();
        }

        public override bool CanProceed()
        {
            return !string.IsNullOrEmpty(stateService.TargetConfigPath) &&
                   !string.IsNullOrEmpty(stateService.SourceConfigPath);
        }

        public override void Refresh()
        {
            stepView.SetTargetChoices(configService.TargetConfigNames);
            stepView.SetSourceChoices(configService.SourceConfigNames);

            stepView.SetTargetIndex(configService.GetTargetConfigIndex(stateService.TargetConfigPath));
            stepView.SetSourceIndex(configService.GetSourceConfigIndex(stateService.SourceConfigPath));

            bool show = configService.CurrentTargetConfigs.Count == 0;
            stepView.SetNoConfigVisibility(show);
        }
    }
}
