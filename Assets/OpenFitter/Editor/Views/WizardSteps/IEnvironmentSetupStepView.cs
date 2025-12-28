using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the Environment Setup step view.
    /// Provides access to UI containers for the presenter to manage setup items.
    /// </summary>
    public interface IEnvironmentSetupStepView
    {
        VisualElement GetSetupStepsContainer();
        void SetSetupAllButtonEnabled(bool enabled);
        void SetResetButtonEnabled(bool enabled);
        void SetSetupAllButtonText(string text);
        ISetupItemView AddSetupItem();
        void ClearSetupItems();

        event System.Action? OnSetupAllClicked;
        event System.Action? OnResetClicked;
    }
}
