#nullable enable
using System;
using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Wizard view for shared UI and navigation.
    /// </summary>
    public sealed class OpenFitterWizardView : IWizardView
    {
        private const string PartsUxmlPath = "Assets/OpenFitter/Editor/Views/WizardStepParts.uxml";

        private readonly VisualElement stepContentContainer;
        private readonly VisualElement stepIndicatorContainer;
        private readonly Label lblStepTitle;
        private readonly Button btnBack;
        private readonly Button btnNext;
        private readonly VisualTreeAsset partsAsset;

        public event NavigationClickHandler? OnNextClicked;
        public event NavigationClickHandler? OnBackClicked;

        public OpenFitterWizardView(VisualElement root)
        {
            stepContentContainer = root.Q<VisualElement>("step-content-container");
            stepIndicatorContainer = root.Q<VisualElement>("step-indicators");
            lblStepTitle = root.Q<Label>("lbl-step-title");
            btnBack = root.Q<Button>("btn-back");
            btnNext = root.Q<Button>("btn-next");
            partsAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PartsUxmlPath);

            btnBack.clicked += () => OnBackClicked?.Invoke();
            btnNext.clicked += () => OnNextClicked?.Invoke();
        }

        public VisualElement GetStepContentContainer() => stepContentContainer;

        public void SetCurrentStep(WizardStep step) =>
            lblStepTitle.text = $"Step {(int)step}: {WizardStepMetadata.GetStepTitle(step)}";

        public void UpdateStepIndicators(WizardStep currentStep)
        {
            stepIndicatorContainer.Clear();
            int totalSteps = WizardStepMetadata.GetTotalSteps();

            for (int i = 0; i < totalSteps; i++)
            {
                var step = (WizardStep)i;
                var state = GetStepState(step, currentStep);
                stepIndicatorContainer.Add(CreateStepIndicator(i, WizardStepMetadata.GetStepTitle(step), state));

                if (i < totalSteps - 1)
                    stepIndicatorContainer.Add(CreateStepArrow());
            }
        }

        public void ClearStepContent() => stepContentContainer.Clear();
        public void SetBackButtonEnabled(bool enabled) => btnBack.SetEnabled(enabled);
        public void SetNextButtonEnabled(bool enabled) => btnNext.SetEnabled(enabled);
        public void SetNextButtonText(string text) => btnNext.text = text;
        public void SetNextButtonVisible(bool visible) =>
            btnNext.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;

        private StepIndicatorState GetStepState(WizardStep step, WizardStep currentStep) =>
            step < currentStep ? StepIndicatorState.Completed :
            step == currentStep ? StepIndicatorState.Current :
            StepIndicatorState.Future;

        private VisualElement CreateStepIndicator(int index, string name, StepIndicatorState state)
        {
            var part = partsAsset.CloneTree().Q<VisualElement>("part-indicator");
            part.RemoveFromHierarchy();

            var circle = part.Q<VisualElement>("step-circle");
            circle.ClearClassList();
            circle.AddToClassList("step-circle"); // Ensure base class if needed, or just clear and add specific
            circle.AddToClassList($"step-{state.ToString().ToLower()}");

            part.Q<Label>("lbl-number").text = index.ToString();
            part.Q<Label>("lbl-name").text = name;
            return part;
        }

        private VisualElement CreateStepArrow()
        {
            var arrow = partsAsset.CloneTree().Q<VisualElement>("part-arrow");
            arrow.RemoveFromHierarchy();
            return arrow;
        }

        private enum StepIndicatorState { Completed, Current, Future }
    }
}

