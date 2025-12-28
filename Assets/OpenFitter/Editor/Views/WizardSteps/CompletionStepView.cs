using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Completion step.
    /// </summary>
    public sealed class CompletionStepView : ICompletionStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/CompletionStep.uxml";

        private readonly VisualElement container;
        private readonly Label lblOutputPath;
        private readonly Button btnViewResult;

        public event System.Action? OnViewResultClicked;

        public CompletionStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            stepAsset.CloneTree(container);

            // Get UI elements
            lblOutputPath = container.Q<Label>("lbl-output-path")!;
            btnViewResult = container.Q<Button>("btn-view-result")!;

            // Bind events
            btnViewResult.clicked += () => OnViewResultClicked?.Invoke();
        }

        public void SetOutputPath(string path)
        {
            lblOutputPath.text = path;
        }
    }
}
