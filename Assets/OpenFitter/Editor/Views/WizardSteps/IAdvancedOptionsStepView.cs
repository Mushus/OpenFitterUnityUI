namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the Advanced Options step view.
    /// </summary>
    public interface IAdvancedOptionsStepView
    {
        void SetSubdivideValue(bool value);
        void SetTriangulateValue(bool value);

        event System.Action<bool>? OnSubdivideChanged;
        event System.Action<bool>? OnTriangulateChanged;
    }
}
