namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Interface for the Completion step view.
    /// </summary>
    public interface ICompletionStepView
    {
        void SetOutputPath(string path);

        event System.Action? OnViewResultClicked;
    }
}
