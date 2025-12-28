#nullable enable
using System;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for a single setup item view.
    /// </summary>
    public interface ISetupItemView
    {
        void SetTitle(string title);
        void SetStatus(string status, string className);
        void SetActionButtonLabel(string label);
        void SetActionButtonEnabled(bool enabled);
        void SetMoreButtonVisible(bool visible);

        event Action OnActionClicked;
        event Action OnMoreClicked;
        event Action OnWebsiteClicked;
    }
}
