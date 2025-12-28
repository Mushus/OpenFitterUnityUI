using UnityEditor;
using UnityEngine;
using System.Linq;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterAssetPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            // Check if any JSON files are affected
            bool jsonChanged = importedAssets.Any(path => path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                               || deletedAssets.Any(path => path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                               || movedAssets.Any(path => path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase))
                               || movedFromAssetPaths.Any(path => path.EndsWith(".json", System.StringComparison.OrdinalIgnoreCase));

            if (jsonChanged) 
            {
                // We don't have a global instance of model, so we'll just trigger the static Refresh of ConfigParser 
                // and potentially find a way to notify open windows.
                // Actually, the easiest is to just let the model refresh when needed, 
                // but since the user wants it automatic, we can force a refresh if the window is open.
                
                var windows = Resources.FindObjectsOfTypeAll<OpenFitterWindow>();
                foreach (var window in windows)
                {
                    window.RefreshProjectJson();
                }
            }
        }
    }
}
