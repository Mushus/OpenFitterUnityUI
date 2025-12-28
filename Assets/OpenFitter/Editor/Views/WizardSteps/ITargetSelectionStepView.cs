using System.Collections.Generic;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the Target Selection step view.
    /// </summary>
    public interface ITargetSelectionStepView
    {
        void SetTargetChoices(List<string> choices);
        void SetSourceChoices(List<string> choices);
        void SetTargetIndex(int index);
        void SetSourceIndex(int index);
        void SetNoConfigVisibility(bool visible);

        event System.Action<int>? OnTargetIndexChanged;
        event System.Action<int>? OnSourceIndexChanged;
    }
}
