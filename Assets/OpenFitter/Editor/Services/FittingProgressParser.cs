using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OpenFitter.Editor.Services
{
    public enum FittingExecutionState
    {
        Idle,
        Processing,
        Completed,
        Error
    }


    public class FittingProgressParser
    {
        private static readonly Regex ProgressRegex = new(@"\((\d+)/(\d+)\)");
        private static readonly Regex StatusDetailRegex = new(@"Status: \[.*\] \(\d+/\d+\) (.*)");

        public struct ParseResult
        {
            public bool HasProgressUpdate;
            public float CurrentProgress; // 0.0 to 1.0 (step progress)
            public string StatusDetail;
        }

        public ParseResult Parse(string log)
        {
            var result = new ParseResult();

            // Extract Progress
            var match = ProgressRegex.Match(log);
            if (match.Success)
            {
                if (float.TryParse(match.Groups[1].Value, out float current) &&
                    float.TryParse(match.Groups[2].Value, out float total) && total > 0)
                {
                    result.HasProgressUpdate = true;
                    // Clamp to 0-1 range just in case
                    float rawProgress = current / total;
                    result.CurrentProgress = rawProgress > 1f ? 1f : (rawProgress < 0f ? 0f : rawProgress);
                }
            }

            // Extract Status Detail
            var detailMatch = StatusDetailRegex.Match(log);
            if (detailMatch.Success)
            {
                result.StatusDetail = detailMatch.Groups[1].Value;
            }

            return result;
        }



        public bool IsContinuousFitting(OpenFitterState state, List<ConfigInfo> availableConfigs)
        {
            ConfigInfo sourceConfig = availableConfigs.Find(c => c.configPath == state.SourceConfigPath);
            ConfigInfo targetConfig = availableConfigs.Find(c => c.configPath == state.TargetConfigPath);

            bool sourceOutputIsTemplate = IsTemplateAvatar(sourceConfig.baseAvatar.name);
            bool targetInputIsTemplate = IsTemplateAvatar(targetConfig.clothingAvatar.name);
            return sourceOutputIsTemplate && targetInputIsTemplate;
        }


        private static bool IsTemplateAvatar(string avatarName)
        {
            if (string.IsNullOrEmpty(avatarName)) return false;
            return avatarName.Equals("Template", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines the execution state based on fitting status and summary string.
        /// </summary>
        public static FittingExecutionState AnalyzeExecutionStateStatic(bool isFitting, string lastRunSummary)
        {
            if (isFitting)
            {
                return FittingExecutionState.Processing;
            }

            if (string.IsNullOrEmpty(lastRunSummary))
            {
                return FittingExecutionState.Idle;
            }

            if (lastRunSummary.Contains("success") || lastRunSummary.Contains("completed"))
            {
                return FittingExecutionState.Completed;
            }

            if (lastRunSummary.Contains("Failed") || lastRunSummary.Contains("Error"))
            {
                return FittingExecutionState.Error;
            }

            return FittingExecutionState.Idle;
        }
    }
}
