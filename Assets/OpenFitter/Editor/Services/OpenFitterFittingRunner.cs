using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using OpenFitter.Editor.Strategies;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterFittingRunner
    {
        private readonly OpenFitterCommandBuilder commandBuilder;
        private readonly OpenFitterCommandRunner commandRunner;
        private readonly FittingProgressParser progressParser = new();

        private Process? currentProcess;
        private IFittingStrategy? currentStrategy;
        private readonly Queue<string> logQueue = new();

        // Context data for strategy
        private OpenFitterState? currentState;
        private List<BlendShapeEntry>? currentBlendShapeEntries;
        private List<ConfigInfo>? currentAvailableConfigs;

        public string? CurrentBlenderPath { get; private set; }
        public string? CurrentScriptPath { get; private set; }

        public OpenFitterFittingRunner(
            OpenFitterCommandBuilder commandBuilder,
            OpenFitterCommandRunner commandRunner)
        {
            this.commandBuilder = commandBuilder ?? throw new ArgumentNullException(nameof(commandBuilder));
            this.commandRunner = commandRunner ?? throw new ArgumentNullException(nameof(commandRunner));
        }

        public bool IsFitting { get; private set; }
        public string LastRunSummary { get; private set; } = "Not run";
        public string LastOutputPath { get; set; } = ""; // Set by strategies

        public int CurrentStep => currentStrategy?.CurrentStep ?? 1;
        public int TotalSteps => currentStrategy?.TotalSteps ?? 1;

        public event StateChangedHandler? OnStateChanged;
        public event LogReceivedHandler? OnLogReceived;
        public event StatusChangedHandler? OnStatusChanged;
        public event StepChangedHandler? OnStepChanged;

        public void Execute(OpenFitterState state, List<BlendShapeEntry> blendShapeEntries, List<ConfigInfo> availableConfigs, string blenderPath, string scriptPath)
        {
            if (IsFitting)
            {
                return;
            }

            currentState = state;
            currentBlendShapeEntries = blendShapeEntries;
            currentAvailableConfigs = availableConfigs;
            CurrentBlenderPath = blenderPath;
            CurrentScriptPath = scriptPath;

            bool isContinuousFitting = progressParser.IsContinuousFitting(state, availableConfigs);

            if (isContinuousFitting)
            {
                currentStrategy = new ContinuousFittingStrategy();
            }
            else
            {
                currentStrategy = new SingleStepFittingStrategy();
            }

            IsFitting = true;
            OnStateChanged?.Invoke();
            OnStepChanged?.Invoke(CurrentStep, TotalSteps);

            currentStrategy.Start(this, state, blendShapeEntries, availableConfigs);

            EditorApplication.update += UpdateFitting;
        }

        public void Cancel()
        {
            if (IsFitting && currentProcess != null && !currentProcess.HasExited)
            {
                try { currentProcess.Kill(); } catch { }
                FinishFitting(false, "Cancelled by user.");
            }
        }

        // Helper methods for strategies


        // GenerateOutputFbxPath wrapper removed. Strategies should use OpenFitterPathUtility directly.


        // GenerateOutputFbxPath wrapper removed. Strategies should use OpenFitterPathUtility directly.

        internal void NotifyStatusChanged(string message)
        {
            OnStatusChanged?.Invoke(message);
            OnStepChanged?.Invoke(CurrentStep, TotalSteps);
        }

        internal void StartProcess(string exe, OpenFitterCoreArguments args)
        {
            try
            {
                string arguments = commandBuilder.BuildArguments(args);
                currentProcess = commandRunner.StartCommand(exe, arguments,
                    data =>
                    {
                        if (data == null) return;
                        lock (logQueue) { logQueue.Enqueue(data); }
                    },
                    error =>
                    {
                        if (error == null) return;
                        lock (logQueue) { logQueue.Enqueue("ERROR: " + error); }
                    }
                );
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[OpenFitter] Process start failed: {ex}");
                FinishFitting(false, $"Failed to start Blender process: {ex.Message}");
            }
        }

        internal void FinishFitting(bool success, string message)
        {
            IsFitting = false;
            currentStrategy = null;
            currentProcess = null;
            EditorApplication.update -= UpdateFitting;

            LastRunSummary = message;
            OnLogReceived?.Invoke($"\n[System] {message}");
            OnStatusChanged?.Invoke(success ? "Completed" : "Failed");

            OnStateChanged?.Invoke();
        }

        private void UpdateFitting()
        {
            lock (logQueue)
            {
                while (logQueue.Count > 0)
                {
                    OnLogReceived?.Invoke(logQueue.Dequeue());
                }
            }

            if (currentProcess == null) return;

            if (currentProcess.HasExited)
            {
                bool success = currentProcess.ExitCode == 0;
                currentProcess.Dispose();
                var finishedProcess = currentProcess;
                currentProcess = null;

                // Delegate completion handling to strategy
                if (currentStrategy != null && currentState != null && currentBlendShapeEntries != null && currentAvailableConfigs != null)
                {
                    currentStrategy.OnProcessExited(this, success, currentState, currentBlendShapeEntries, currentAvailableConfigs);
                }
                else
                {
                    // Fallback should typically not happen
                    FinishFitting(success, success ? "Completed" : "Process exited.");
                }
            }
        }

        public float CalculateOverallProgress(float stepProgress)
        {
            return currentStrategy?.CalculateOverallProgress(stepProgress) ?? stepProgress;
        }
    }
}
