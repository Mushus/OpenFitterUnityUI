using System.Collections.Generic;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor.Strategies
{
    public class SingleStepFittingStrategy : IFittingStrategy
    {
        public int CurrentStep => 1;
        public int TotalSteps => 1;

        public void Start(OpenFitterFittingRunner runner, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            var configForOutput = availableConfigs.Find(c => c.configPath == state.SourceConfigPath)
                               ?? availableConfigs.Find(c => c.configPath == state.TargetConfigPath);

            // Must have a config for base properties
            if (configForOutput == null)
            {
                runner.FinishFitting(false, "No valid configuration found for fitting.");
                return;
            }

            string? blenderPath = runner.CurrentBlenderPath;
            string? scriptPath = runner.CurrentScriptPath;

            if (blenderPath == null || scriptPath == null)
            {
                runner.FinishFitting(false, "Blender path or script path is missing.");
                return;
            }

            var coreArgs = OpenFitterCoreArguments.FromState(state, configForOutput, scriptPath);

            // Apply blend shapes from entries (overrides settings string if present)
            coreArgs.BlendShapeEntries = new List<BlendShapeEntry>(blendShapeEntries);

            string outputPath = OpenFitterPathUtility.GenerateOutputFbxPath(configForOutput);
            coreArgs.OutputFbx = outputPath; // Set output path
            runner.LastOutputPath = outputPath;

            string desc = configForOutput.displayName;
            runner.NotifyStatusChanged($"Exec Single Step: {desc}");

            runner.StartProcess(blenderPath, coreArgs);
        }

        public void OnProcessExited(OpenFitterFittingRunner runner, bool success, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            if (success)
            {
                runner.FinishFitting(true, "Fitting completed successfully.");
            }
            else
            {
                runner.FinishFitting(false, "Process exited with error code.");
            }
        }

        public float CalculateOverallProgress(float stepProgress) => stepProgress;
    }
}
