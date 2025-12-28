namespace OpenFitter.Editor
{
    /// <summary>
    /// Interface for environment setup steps (downloaders, installers, etc.).
    /// </summary>
    public interface ISetupStep
    {
        /// <summary>
        /// Checks if the step is complete and ready.
        /// </summary>
        bool IsReady { get; }

        /// <summary>
        /// Checks if the step is currently in progress.
        /// </summary>
        bool IsInProgress { get; }

        /// <summary>
        /// Gets the current progress (0 to 1).
        /// </summary>
        float Progress { get; }
    }
}
