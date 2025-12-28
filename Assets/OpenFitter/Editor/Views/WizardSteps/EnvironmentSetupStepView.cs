using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Environment Setup step.
    /// </summary>
    public sealed class EnvironmentSetupStepView : IEnvironmentSetupStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/EnvironmentSetupStep.uxml";

        private readonly VisualElement container;
        private readonly Button btnSetupAll;
        private readonly Button btnResetEnvironment;
        private readonly VisualElement listSetupSteps;
        private readonly VisualTreeAsset itemTemplate;

        public event System.Action? OnSetupAllClicked;
        public event System.Action? OnResetClicked;

        public EnvironmentSetupStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            stepAsset.CloneTree(container);

            // Get UI elements
            btnSetupAll = container.Q<Button>("btn-download-all")!;
            btnResetEnvironment = container.Q<Button>("btn-reset-environment")!;
            listSetupSteps = container.Q<VisualElement>("list-setup-steps")!;
            itemTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/OpenFitter/Editor/Views/OpenFitterSetupStep.uxml")!;

            // Bind events
            btnSetupAll.clicked += () => OnSetupAllClicked?.Invoke();
            btnResetEnvironment.clicked += () => OnResetClicked?.Invoke();
        }

        public VisualElement GetSetupStepsContainer()
        {
            return listSetupSteps;
        }

        public void SetSetupAllButtonEnabled(bool enabled)
        {
            btnSetupAll.SetEnabled(enabled);
        }

        public void SetResetButtonEnabled(bool enabled)
        {
            btnResetEnvironment.SetEnabled(enabled);
        }

        public void SetSetupAllButtonText(string text)
        {
            btnSetupAll.text = text;
        }

        public ISetupItemView AddSetupItem()
        {
            var instance = itemTemplate.CloneTree();
            listSetupSteps.Add(instance);
            return new SetupItemView(instance);
        }

        public void ClearSetupItems()
        {
            listSetupSteps.Clear();
        }
    }
}
