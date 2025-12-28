using UnityEditor;

namespace OpenFitter.Editor.Services
{
    public static class SetupResultHandler
    {
        public static void HandleError(SetupResult result, string taskName)
        {
            if (result.IsFailed)
            {
                EditorUtility.DisplayDialog(taskName, result.Message, I18n.Tr("OK"));
            }
            EditorUtility.ClearProgressBar();
        }

        public static void UpdateProgressBar(SetupResult result, string taskName, float progress)
        {
            if (result.IsInProgress)
            {
                EditorUtility.DisplayProgressBar(
                    I18n.Tr("Processing..."),
                    $"{taskName}: {result.Message ?? ""}",
                    progress
                );
            }
            else
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
