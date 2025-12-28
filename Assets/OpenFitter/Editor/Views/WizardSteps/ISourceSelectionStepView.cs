using UnityEngine;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the Source Selection step view.
    /// </summary>
    public interface ISourceSelectionStepView
    {
        void SetSourceFbxValue(GameObject? obj);

        event System.Action<Object?>? OnSourceFbxChanged;
        event System.Action? OnSelectFileClicked;
    }
}
