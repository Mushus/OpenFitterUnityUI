#nullable enable
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Target Selection step.
    /// </summary>
    public sealed class TargetSelectionStepView : ITargetSelectionStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/TargetSelectionStep.uxml";

        private readonly VisualElement container;
        private readonly DropdownField dropdownTarget;
        private readonly DropdownField dropdownSource;
        private readonly HelpBox boxNoConfig;

        public event System.Action<int>? OnTargetIndexChanged;
        public event System.Action<int>? OnSourceIndexChanged;

        public TargetSelectionStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            if (stepAsset != null)
            {
                stepAsset.CloneTree(container);
            }

            // Get UI elements
            dropdownTarget = container.Q<DropdownField>("dropdown-target")!;
            dropdownSource = container.Q<DropdownField>("dropdown-source")!;
            boxNoConfig = container.Q<HelpBox>("box-no-config")!;

            // Bind events
            dropdownTarget.RegisterValueChangedCallback(evt => OnTargetIndexChanged?.Invoke(dropdownTarget.index));
            dropdownSource.RegisterValueChangedCallback(evt => OnSourceIndexChanged?.Invoke(dropdownSource.index));
        }

        public void SetTargetChoices(List<string> choices)
        {
            dropdownTarget.choices = choices;
        }

        public void SetSourceChoices(List<string> choices)
        {
            dropdownSource.choices = choices;
        }

        public void SetTargetIndex(int index)
        {
            dropdownTarget.index = index;
        }

        public void SetSourceIndex(int index)
        {
            dropdownSource.index = index;
        }

        public void SetNoConfigVisibility(bool visible)
        {
            boxNoConfig.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
