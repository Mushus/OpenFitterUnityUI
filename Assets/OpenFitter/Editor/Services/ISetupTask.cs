using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    public interface ISetupTask
    {
        bool IsRunning { get; }
        bool IsReady { get; }
        float Progress { get; }

        SetupResult Start();
        SetupResult Update();
        void Abort();
    }
}
