#nullable enable
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class BlendShapeStepPresenter : WizardStepPresenterBase
    {
        private readonly BlendShapeStepView stepView;
        private readonly ConfigurationService configService;

        public BlendShapeStepPresenter(
            OpenFitterState stateService,
            ConfigurationService configService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            this.configService = configService;

            // Load logic moved from OnEnter
            if (stateService.BlendShapeEntries.Count == 0)
            {
                ConfigInfo? sourceConfig = configService.GetConfigByPath(stateService.SourceConfigPath);
                if (sourceConfig != null)
                {
                    var entries = configService.LoadBlendShapes(sourceConfig);
                    stateService.SetBlendShapeEntries(entries);
                }
            }

            stepView = new BlendShapeStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            stepView.OnEntryEnabledChanged += OnEntryEnabledChanged;
            stepView.OnEntryCustomNameChanged += OnEntryCustomNameChanged;
            stepView.OnEntryValueChanged += OnEntryValueChanged;
        }

        protected override void UnbindElements()
        {
            stepView.OnEntryEnabledChanged -= OnEntryEnabledChanged;
            stepView.OnEntryCustomNameChanged -= OnEntryCustomNameChanged;
            stepView.OnEntryValueChanged -= OnEntryValueChanged;
        }

        private void OnEntryEnabledChanged(int index, bool enabled)
        {
            if (index >= 0 && index < stateService.BlendShapeEntries.Count)
            {
                stateService.BlendShapeEntries[index].enabled = enabled;
            }
        }

        private void OnEntryCustomNameChanged(int index, string customName)
        {
            if (index >= 0 && index < stateService.BlendShapeEntries.Count)
            {
                stateService.BlendShapeEntries[index].customName = customName;
            }
        }

        private void OnEntryValueChanged(int index, float value)
        {
            if (index >= 0 && index < stateService.BlendShapeEntries.Count)
            {
                stateService.BlendShapeEntries[index].value = value;
            }
        }

        public override bool CanProceed()
        {
            return true;
        }

        public override void Refresh()
        {
            stepView.RenderBlendShapeList(stateService.BlendShapeEntries);
        }
    }
}
