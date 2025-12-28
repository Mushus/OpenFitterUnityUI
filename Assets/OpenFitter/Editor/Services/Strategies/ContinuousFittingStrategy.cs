using System;
using System.Collections.Generic;
using System.IO; // Kept this as it's used in the class and not explicitly removed by the provided 'Code Edit' block's using directives, which seemed to be an excerpt.
using OpenFitter.Editor; // Kept this as it's used in the class and not explicitly removed by the provided 'Code Edit' block's using directives, which seemed to be an excerpt.
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor.Strategies
{
    public class ContinuousFittingStrategy : IFittingStrategy
    {
        private int fittingStep = 0;
        public int CurrentStep => fittingStep;
        public int TotalSteps => 2;
        private ConfigInfo? pendingTargetConfig;
        private string pendingSourceOutput = string.Empty;

        public void Start(OpenFitterFittingRunner runner, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            ConfigInfo? sourceConfig = availableConfigs.Find(c => c.configPath == state.SourceConfigPath);
            ConfigInfo? targetConfig = availableConfigs.Find(c => c.configPath == state.TargetConfigPath);

            // Step 1
            fittingStep = 1;
            pendingTargetConfig = targetConfig;

            // Step 1: Fitting Source -> Target
            // For Step 1 base args, we use SourceConfig (if valid) or TargetConfig? 
            // Actually Step 1 is fitting 'Clothing'(Input) to 'Base'(Avatar).
            // Input usually overrides Clothing.
            // Base comes from Config.
            // In Continuous, usually Step 1 is "Scale Input to Target Base". 
            // Wait, standard continuous flow:
            // 1. Fit Source(Clothing) to Target(Base)? No.
            // Usually: Fit Input(Clothing) to Intermediate or similar?
            // Let's assume Step 1 uses TargetConfig's Base as reference? 
            // Or if SourceConfig is present, maybe use that?
            // "Apply Source Config (force override)" logic was present.
            // Let's use SourceConfig if available, else TargetConfig.

            var step1Config = sourceConfig ?? targetConfig;
            if (step1Config == null)
            {
                runner.FinishFitting(false, "No valid configuration found for Step 1.");
                return;
            }

            string? blenderPath = runner.CurrentBlenderPath;
            string? scriptPath = runner.CurrentScriptPath;

            if (blenderPath == null || scriptPath == null)
            {
                runner.FinishFitting(false, "Blender path or script path is missing.");
                return;
            }

            // Build CoreArgs for Step 1
            var coreArgs = OpenFitterCoreArguments.FromState(state, step1Config, scriptPath);
            // In step 1, we don't apply blend shapes and disable extra processing
            coreArgs.Subdivide = false;
            coreArgs.Triangulate = false;

            // Apply Source Config (force override)
            // Apply Source Config (force override) - Now handled in CreateCoreArguments if passed as step1Config
            // But we keep generation of output path logic
            if (sourceConfig != null)
            {
                // runner.ApplyConfigToCoreArgs(sourceConfig, coreArgs); // Redundant if passed to CreateCoreArguments
                pendingSourceOutput = OpenFitterPathUtility.GenerateOutputFbxPath(sourceConfig);
            }
            else
            {
                // Should potentially error out if source config missing for continuous?
                pendingSourceOutput = "output_step1.fbx";
            }

            coreArgs.OutputFbx = pendingSourceOutput;

            // Pass empty list for Step 1 - use config's own BlendShape settings
            // We do NOT call UpdateCoreArgsWithBlendShapes(coreArgs, blendShapeEntries) here.

            string desc = sourceConfig != null ? sourceConfig.displayName : "Step 1";
            runner.NotifyStatusChanged($"Step 1/{TotalSteps}: {desc}");

            runner.StartProcess(blenderPath, coreArgs);
        }

        public void OnProcessExited(OpenFitterFittingRunner runner, bool success, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            if (!success)
            {
                runner.FinishFitting(false, "Process exited with error code.");
                return;
            }

            if (fittingStep == 1)
            {
                StartStep2(runner, state, blendShapeEntries, availableConfigs);
            }
            else
            {
                runner.FinishFitting(true, "Fitting completed successfully.");
            }
        }

        private void StartStep2(OpenFitterFittingRunner runner, OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            fittingStep = 2;

            if (!File.Exists(pendingSourceOutput))
            {
                runner.FinishFitting(false, "Step 1 output file not found: " + pendingSourceOutput);
                return;
            }

            // Build CoreArgs for Step 2
            // Step 2 uses pendingTargetConfig (Target) as the define for Base Avatar to fit TO.
            if (pendingTargetConfig == null)
            {
                runner.FinishFitting(false, "No valid target configuration for Step 2.");
                return;
            }

            string? blenderPath2 = runner.CurrentBlenderPath;
            string? scriptPath2 = runner.CurrentScriptPath;

            if (blenderPath2 == null || scriptPath2 == null)
            {
                runner.FinishFitting(false, "Blender path or script path is missing.");
                return;
            }

            var coreArgs = OpenFitterCoreArguments.FromState(state, pendingTargetConfig, scriptPath2);

            // Use Step 1 Output as Step 2 Input
            coreArgs.InputFbx = pendingSourceOutput;

            // Apply Target Config
            // Apply Target Config - Handled by CreateCoreArguments(..., pendingTargetConfig)
            // if (pendingTargetConfig != null)
            // {
            //    runner.ApplyConfigToCoreArgs(pendingTargetConfig, coreArgs);
            // }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string outputPathBase = OpenFitterPathUtility.GenerateOutputFbxPath(pendingTargetConfig);
            string outputDir = Path.GetDirectoryName(outputPathBase) ?? "";
            string sourceName = GetConfigName(availableConfigs, state.SourceConfigPath);

            // Construct new filename based on Source -> Target
            string filename = $"{sourceName}_to_{pendingTargetConfig.clothingAvatar.name}_{timestamp}.fbx";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            string finalOutputPath = Path.Combine(outputDir, filename);
            runner.LastOutputPath = finalOutputPath;
            coreArgs.OutputFbx = finalOutputPath;

            // Apply user blend shapes for Step 2
            coreArgs.BlendShapeEntries = new List<BlendShapeEntry>(blendShapeEntries);

            string desc = pendingTargetConfig.displayName;
            runner.NotifyStatusChanged($"Step 2/{TotalSteps}: {desc}");

            runner.StartProcess(blenderPath2, coreArgs);
        }

        public float CalculateOverallProgress(float stepProgress)
        {
            return (fittingStep - 1 + stepProgress) / TotalSteps;
        }

        private static string GetConfigName(List<ConfigInfo> availableConfigs, string path)
        {
            var c = availableConfigs.Find(x => x.configPath == path);
            return c != null ? c.clothingAvatar.name : "Unknown";
        }
    }
}
