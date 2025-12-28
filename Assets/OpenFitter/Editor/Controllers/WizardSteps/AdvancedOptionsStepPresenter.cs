using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class AdvancedOptionsStepPresenter : WizardStepPresenterBase
    {
        private readonly AdvancedOptionsStepView stepView;

        public AdvancedOptionsStepPresenter(
            OpenFitterState stateService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            stepView = new AdvancedOptionsStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            stepView.OnSubdivideChanged += OnSubdivideChanged;
            stepView.OnTriangulateChanged += OnTriangulateChanged;
        }

        protected override void UnbindElements()
        {
            stepView.OnSubdivideChanged -= OnSubdivideChanged;
            stepView.OnTriangulateChanged -= OnTriangulateChanged;
        }

        private void OnSubdivideChanged(bool value)
        {
            stateService.Subdivide = value;
        }

        private void OnTriangulateChanged(bool value)
        {
            stateService.Triangulate = value;
        }

        public override bool CanProceed()
        {
            return true;
        }

        public override void Refresh()
        {
            stepView.SetSubdivideValue(stateService.Subdivide);
            stepView.SetTriangulateValue(stateService.Triangulate);
        }
    }
}
