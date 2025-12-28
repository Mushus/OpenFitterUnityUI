using System.Collections.Generic;

namespace OpenFitter.Editor.Services
{

    public class FittingService
    {
        private readonly OpenFitterFittingRunner fittingRunner;

        public event LogReceivedHandler? OnLogReceived;
        public event StatusChangedHandler? OnStatusChanged;
        public event StepChangedHandler? OnStepChanged;
        public event ProgressChangedHandler? OnProgressChanged;
        public event StateChangedHandler? OnStateChanged;

        public string LastRunSummary => fittingRunner.LastRunSummary;
        public string LastOutputPath => fittingRunner.LastOutputPath;
        public bool IsFitting => fittingRunner.IsFitting;

        private readonly FittingProgressParser progressParser;

        private readonly IOpenFitterEnvironmentService environmentService;

        public FittingService(IOpenFitterEnvironmentService environmentService)
        {
            this.environmentService = environmentService;
            var commandRunner = new OpenFitterCommandRunner();
            var commandBuilder = new OpenFitterCommandBuilder();
            progressParser = new FittingProgressParser();

            fittingRunner = new OpenFitterFittingRunner(
                commandBuilder,
                commandRunner);

            fittingRunner.OnLogReceived += HandleLogReceived;
            fittingRunner.OnStatusChanged += text => OnStatusChanged?.Invoke(text);
            fittingRunner.OnStepChanged += (current, total) => OnStepChanged?.Invoke(current, total);
            fittingRunner.OnStateChanged += () => OnStateChanged?.Invoke();
        }

        private void HandleLogReceived(string log)
        {
            OnLogReceived?.Invoke(log);
            ParseProgress(log);
        }

        private void ParseProgress(string log)
        {
            var result = progressParser.Parse(log);
            if (result.HasProgressUpdate || !string.IsNullOrEmpty(result.StatusDetail))
            {
                // Calculate overall progress if update available, otherwise just pass 0 or keep current? 
                // Ideally we pass specific valid progress. 
                // If only status changed, progress might be same.
                // Let's calculate it anyway if we have step info.

                float overall = 0f;
                // Note: CurrentStep/TotalSteps in fittingRunner might be updated via OnStepChanged events, 
                // but might not be perfectly synced with this log line if they come from different parsing streams?
                // Actually OpenFitterFittingRunner manages CurrentStep.

                if (result.HasProgressUpdate)
                {
                    overall = fittingRunner.CalculateOverallProgress(result.CurrentProgress);
                }

                OnProgressChanged?.Invoke(overall, result.StatusDetail);
            }
        }

        public void ExecuteFitting(OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs)
        {
            if (!ValidateEnvironmentForRun(state))
            {
                UnityEngine.Debug.LogError("[OpenFitter] Environment is not ready for fitting.");
                return;
            }

            UnityEngine.Debug.Log("[OpenFitter] ExecuteFitting called");
            fittingRunner.Execute(state, blendShapeEntries, availableConfigs, environmentService.BlenderPath, environmentService.ScriptPath);
        }

        public void CancelFitting()
        {
            fittingRunner.Cancel();
        }

        private bool ValidateEnvironmentForRun(OpenFitterState state)
        {
            // Moved basic validation logic here if possible, or delegate back to EnvironmentService if needed.
            // For now, checks are done before calling this in original code, but we can double check.
            if (string.IsNullOrEmpty(state.SourceConfigPath) || string.IsNullOrEmpty(state.TargetConfigPath))
            {
                return false;
            }
            return true;
        }

        // Forward cleanup
        public void Dispose()
        {
            // If fittingRunner needs disposal
        }
    }
}


