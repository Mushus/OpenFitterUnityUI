using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using OpenFitter.Editor.Services;
using OpenFitter.Editor.Views;

namespace OpenFitter.Editor
{
    public sealed class CompletionStepPresenter : WizardStepPresenterBase
    {
        private readonly CompletionStepView stepView;

        public CompletionStepPresenter(
            OpenFitterState stateService,
            OpenFitterWizardView view,
            OpenFitterWizardPresenter presenter,
            VisualElement parent)
            : base(stateService, view, presenter, parent)
        {
            stepView = new CompletionStepView(stepContainer);
            BindElements();
        }

        protected override string UxmlPath => ""; // View handles UXML loading

        protected override void BindElements()
        {
            stepView.OnViewResultClicked += OnViewResultClicked;
        }

        protected override void UnbindElements()
        {
            stepView.OnViewResultClicked -= OnViewResultClicked;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Refresh();
        }

        public override void Refresh()
        {
            stepView.SetOutputPath(stateService.LastOutputPath);
        }

        public override bool CanProceed()
        {
            return true;
        }

        public override bool CanGoBack()
        {
            return true;
        }

        private void OnViewResultClicked()
        {
            string path = stateService.LastOutputPath;
            if (string.IsNullOrEmpty(path)) return;

            AssetDatabase.Refresh();

            string relativePath = OpenFitterPathUtility.ToRelativePath(path);

            Debug.Log($"[OpenFitter] Highlighting file: {relativePath} (Original: {path})");

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(relativePath);
            if (obj != null)
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            }
            else
            {
                Debug.LogWarning($"[OpenFitter] Could not find output file at: {path}");
            }
        }
    }
}
