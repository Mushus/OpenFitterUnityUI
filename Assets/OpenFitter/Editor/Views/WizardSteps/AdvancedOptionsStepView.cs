using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Advanced Options step.
    /// </summary>
    public sealed class AdvancedOptionsStepView : IAdvancedOptionsStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/AdvancedOptionsStep.uxml";

        private readonly VisualElement container;
        private readonly Toggle toggleSubdiv;
        private readonly Toggle toggleTriangulate;

        public event System.Action<bool>? OnSubdivideChanged;
        public event System.Action<bool>? OnTriangulateChanged;

        public AdvancedOptionsStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            stepAsset.CloneTree(container);

            // Get UI elements
            toggleSubdiv = container.Q<Toggle>("toggle-subdiv")!;
            toggleTriangulate = container.Q<Toggle>("toggle-triangulate")!;

            // Bind events
            toggleSubdiv.RegisterValueChangedCallback(evt => OnSubdivideChanged?.Invoke(evt.newValue));
            toggleTriangulate.RegisterValueChangedCallback(evt => OnTriangulateChanged?.Invoke(evt.newValue));
        }

        public void SetSubdivideValue(bool value)
        {
            toggleSubdiv.SetValueWithoutNotify(value);
        }

        public void SetTriangulateValue(bool value)
        {
            toggleTriangulate.SetValueWithoutNotify(value);
        }
    }
}
