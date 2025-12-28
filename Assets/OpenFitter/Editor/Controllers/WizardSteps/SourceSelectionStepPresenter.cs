#nullable enable
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class SourceSelectionStepPresenter : WizardStepPresenterBase
    {
        private readonly SourceSelectionStepView stepView;
        private GameObject? SelectedGameObject => stateService.InputFbxObject;

        public SourceSelectionStepPresenter(
            OpenFitterState stateService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            stepView = new SourceSelectionStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            stepView.OnSourceFbxChanged += OnSourceFbxChanged;
            stepView.OnSelectFileClicked += OnSelectFileClicked;
        }

        protected override void UnbindElements()
        {
            stepView.OnSourceFbxChanged -= OnSourceFbxChanged;
            stepView.OnSelectFileClicked -= OnSelectFileClicked;
        }

        private void OnSourceFbxChanged(Object? obj)
        {
            var gameObj = obj as GameObject;
            stateService.InputFbxObject = gameObj;
            InvokeStatusChanged();
        }

        public override bool CanProceed()
        {
            return SelectedGameObject != null && !string.IsNullOrEmpty(stateService.InputFbx);
        }

        public override void Refresh()
        {
            var currentObj = stateService.InputFbxObject;
            stepView.SetSourceFbxValue(currentObj);
        }

        private void OnSelectFileClicked()
        {
            string dir = OpenFitterPathUtility.ResolveDirectory(stateService.InputFbx);
            string selected = EditorUtility.OpenFilePanel(I18n.Tr("Select Outfit FBX File"), dir, "fbx");
            SetInputFbx(selected);
        }

        private void SetInputFbx(string absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return;

            string relPath = OpenFitterPathUtility.ToRelativePath(absolutePath);
            stateService.InputFbx = relPath;

            stepView.SetSourceFbxValue(stateService.InputFbxObject);
            InvokeStatusChanged();
        }
    }
}
