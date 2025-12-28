#nullable enable
using UnityEditor;
using UnityEngine.UIElements;

namespace OpenFitter.Editor.Views
{
    /// <summary>
    /// Root view for the OpenFitter window.
    /// Responsible for loading and assembling the main window UI structure.
    /// </summary>
    public sealed class OpenFitterRootView : IRootView
    {
        private const string WindowUxmlPath = "Assets/OpenFitter/Editor/Views/OpenFitterWindow.uxml";
        private const string WizardShellUxmlPath = "Assets/OpenFitter/Editor/Views/WizardShell.uxml";

        private readonly VisualElement rootElement;
        private readonly VisualElement wizardHost;

        public OpenFitterRootView(VisualElement rootVisualElement)
        {
            rootElement = rootVisualElement;

            // Load and clone the main window template
            var windowVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(WindowUxmlPath);
            windowVisualTree.CloneTree(rootElement);

            // Get the wizard host container
            wizardHost = rootElement.Q<VisualElement>("wizard-host");

            // Load and clone the wizard shell into the host
            var wizardShellVisualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(WizardShellUxmlPath);
            wizardShellVisualTree.CloneTree(wizardHost);
        }

        /// <summary>
        /// Gets the wizard host container where the wizard UI is loaded.
        /// </summary>
        public VisualElement GetWizardHost() => wizardHost;
    }
}
