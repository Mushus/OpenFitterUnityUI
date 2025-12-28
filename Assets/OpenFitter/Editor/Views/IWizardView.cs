#nullable enable
using UnityEngine.UIElements;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Interface that the Wizard View exposes to the Presenter.
    /// </summary>
    public interface IWizardView
    {
        event NavigationClickHandler? OnNextClicked;
        event NavigationClickHandler? OnBackClicked;

        VisualElement GetStepContentContainer();
        void SetCurrentStep(WizardStep step);
        void UpdateStepIndicators(WizardStep currentStep);
        void ClearStepContent();
        void SetBackButtonEnabled(bool enabled);
        void SetNextButtonEnabled(bool enabled);
        void SetNextButtonText(string text);
        void SetNextButtonVisible(bool visible);
    }
}
