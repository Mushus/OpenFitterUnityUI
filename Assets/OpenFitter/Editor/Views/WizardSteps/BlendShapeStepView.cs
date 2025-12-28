using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the BlendShape step.
    /// </summary>
    public sealed class BlendShapeStepView : IBlendShapeStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/BlendShapeStep.uxml";
        private const string RowUxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/BlendShapeRow.uxml";

        private readonly VisualElement container;
        private readonly VisualElement listBlendShapes;
        private readonly VisualTreeAsset rowAsset;

        public event System.Action<int, bool>? OnEntryEnabledChanged;
        public event System.Action<int, string>? OnEntryCustomNameChanged;
        public event System.Action<int, float>? OnEntryValueChanged;

        public BlendShapeStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            if (stepAsset != null)
            {
                stepAsset.CloneTree(container);
            }

            // Get UI elements
            listBlendShapes = container.Q<VisualElement>("list-blendshapes")!;

            // Load row template
            rowAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(RowUxmlPath);
        }

        public void RenderBlendShapeList(List<BlendShapeEntry> entries)
        {
            listBlendShapes.Clear();

            for (int i = 0; i < entries.Count; i++)
            {
                int index = i;
                var entry = entries[i];

                var row = rowAsset.CloneTree();

                var toggle = row.Q<Toggle>("toggle-enabled")!;
                var originalNameLabel = row.Q<Label>("lbl-original-name")!;
                var nameField = row.Q<TextField>("field-custom-name")!;
                var slider = row.Q<Slider>("slider-value")!;
                var valueField = row.Q<FloatField>("field-value")!;

                // Set values
                toggle.SetValueWithoutNotify(entry.enabled);
                originalNameLabel.text = entry.originalName;
                nameField.SetValueWithoutNotify(entry.customName);
                slider.SetValueWithoutNotify(entry.value);
                valueField.SetValueWithoutNotify(entry.value);

                // Setup enable states
                slider.SetEnabled(entry.enabled);
                valueField.SetEnabled(entry.enabled);

                // Register Callbacks
                originalNameLabel.RegisterCallback<ClickEvent>(_ => toggle.value = !toggle.value);

                toggle.RegisterValueChangedCallback(evt =>
                {
                    OnEntryEnabledChanged?.Invoke(index, evt.newValue);
                    slider.SetEnabled(evt.newValue);
                    valueField.SetEnabled(evt.newValue);
                });

                nameField.RegisterValueChangedCallback(evt =>
                    OnEntryCustomNameChanged?.Invoke(index, evt.newValue));

                slider.RegisterValueChangedCallback(evt =>
                {
                    valueField.SetValueWithoutNotify(evt.newValue);
                    OnEntryValueChanged?.Invoke(index, evt.newValue);
                });

                valueField.RegisterValueChangedCallback(evt =>
                {
                    slider.SetValueWithoutNotify(evt.newValue);
                    OnEntryValueChanged?.Invoke(index, evt.newValue);
                });

                listBlendShapes.Add(row);
            }
        }
    }
}
