#nullable enable
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface that the Root View exposes to the Presenter.
    /// </summary>
    public interface IRootView
    {
        VisualElement GetWizardHost();
    }
}
