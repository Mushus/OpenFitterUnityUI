using System.Collections.Generic;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor.Strategies
{
    public interface IFittingStrategy
    {
        int CurrentStep { get; }
        int TotalSteps { get; }
        void Start(OpenFitterFittingRunner runner, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs);
        void OnProcessExited(OpenFitterFittingRunner runner, bool success, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs);
        float CalculateOverallProgress(float stepProgress);
    }
}
