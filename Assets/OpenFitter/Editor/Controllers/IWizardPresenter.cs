#nullable enable

namespace OpenFitter.Editor
{
    /// <summary>
    /// Interface that the Wizard Presenter exposes to child step presenters.
    /// </summary>
    public interface IWizardPresenter
    {
        void GoNext();
        void GoBack();
        void NavigateTo(WizardStep step);
        void NotifyStatusChanged();
    }
}
