using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Source Selection step.
    /// </summary>
    public sealed class SourceSelectionStepView : ISourceSelectionStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/SourceSelectionStep.uxml";

        private readonly VisualElement container;
        private readonly ObjectField fieldSourceFbx;
        private readonly Button btnSelectFile;

        public event System.Action<Object?>? OnSourceFbxChanged;
        public event System.Action? OnSelectFileClicked;

        public SourceSelectionStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            stepAsset.CloneTree(container);

            // Get UI elements
            fieldSourceFbx = container.Q<ObjectField>("field-source-fbx")!;
            btnSelectFile = container.Q<Button>("btn-select-file")!;

            // Configure ObjectField
            fieldSourceFbx.objectType = typeof(GameObject);

            // Bind events
            fieldSourceFbx.RegisterValueChangedCallback(evt => OnSourceFbxChanged?.Invoke(evt.newValue));
            btnSelectFile.clicked += () => OnSelectFileClicked?.Invoke();
        }

        public void SetSourceFbxValue(GameObject? obj)
        {
            fieldSourceFbx.SetValueWithoutNotify(obj);
        }
    }
}
