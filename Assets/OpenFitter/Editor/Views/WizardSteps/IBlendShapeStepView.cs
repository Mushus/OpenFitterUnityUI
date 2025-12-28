using System.Collections.Generic;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the BlendShape step view.
    /// </summary>
    public interface IBlendShapeStepView
    {
        void RenderBlendShapeList(List<BlendShapeEntry> entries);

        event System.Action<int, bool>? OnEntryEnabledChanged;
        event System.Action<int, string>? OnEntryCustomNameChanged;
        event System.Action<int, float>? OnEntryValueChanged;
    }
}
