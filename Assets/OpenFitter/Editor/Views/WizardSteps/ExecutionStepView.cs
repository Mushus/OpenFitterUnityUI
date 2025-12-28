using System.Text;
using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// View for the Execution step.
    /// </summary>
    public sealed class ExecutionStepView : IExecutionStepView
    {
        private const string UxmlPath = "Assets/OpenFitter/Editor/Views/WizardSteps/ExecutionStep.uxml";

        private readonly VisualElement container;
        private readonly Label lblStatus;
        private readonly Label lblStatusBadge;
        private readonly ProgressBar progressBar;
        private readonly TextField lblLog;
        private readonly ScrollView logScrollView;
        private readonly StringBuilder cumulativeLog = new();

        public ExecutionStepView(VisualElement parentContainer)
        {
            container = parentContainer;

            // Load UXML
            var stepAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(UxmlPath);
            stepAsset.CloneTree(container);

            // Get UI elements
            lblStatus = container.Q<Label>("lbl-status")!;
            lblStatusBadge = container.Q<Label>("lbl-status-badge")!;
            progressBar = container.Q<ProgressBar>("progress-bar")!;
            lblLog = container.Q<TextField>("lbl-log")!;
            logScrollView = container.Q<ScrollView>("log-scroll-view")!;
        }

        public void SetStatus(string status)
        {
            lblStatus.text = status;
        }

        public void SetStatusBadge(string text, string cssClass)
        {
            lblStatusBadge.RemoveFromClassList("processing");
            lblStatusBadge.RemoveFromClassList("completed");
            lblStatusBadge.RemoveFromClassList("failed");

            lblStatusBadge.text = text;
            if (!string.IsNullOrEmpty(cssClass))
            {
                lblStatusBadge.AddToClassList(cssClass);
            }
        }

        public void SetProgress(float progress, string title)
        {
            progressBar.value = progress;
            progressBar.title = title;
        }

        public void AppendLog(string log)
        {
            cumulativeLog.AppendLine(log);
            lblLog.value = cumulativeLog.ToString();
        }

        public void ClearLog()
        {
            cumulativeLog.Clear();
            lblLog.value = string.Empty;
        }

        public void ScrollLogToBottom()
        {
            logScrollView.verticalScroller.value = logScrollView.verticalScroller.highValue;
        }
    }
}
