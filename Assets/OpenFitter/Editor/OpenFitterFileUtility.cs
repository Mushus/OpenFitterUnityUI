using System;
using System.IO;

namespace OpenFitter.Editor
{
    public static class OpenFitterFileUtility
    {
        public static void ClearCacheFiles()
        {
            string blenderToolsDir = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            if (Directory.Exists(blenderToolsDir))
            {
                try
                {
                    Directory.Delete(blenderToolsDir, true);
                    string metaFile = blenderToolsDir + ".meta";
                    if (File.Exists(metaFile)) File.Delete(metaFile);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"[OpenFitter] Failed to delete BlenderTools: {e.Message}");
                }
            }

            string tempRoot = Path.Combine(Path.GetTempPath(), "OpenFitter");
            if (Directory.Exists(tempRoot))
            {
                try
                {
                    Directory.Delete(tempRoot, true);
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"[OpenFitter] Failed to delete Temp files: {e.Message}");
                }
            }
        }

        public static void DeleteBlenderFiles()
        {
            string blenderToolsDir = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            string blenderDir = Path.Combine(blenderToolsDir, OpenFitterConstants.BlenderFolderName);
            string blenderZip = Path.Combine(blenderToolsDir, OpenFitterConstants.BlenderZipFileName);

            TryDeleteDirectory(blenderDir, "Blender");
            TryDeleteFile(blenderZip, "Blender zip");
        }

        public static void DeleteCoreFiles()
        {
            string blenderToolsDir = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            if (Directory.Exists(blenderToolsDir))
            {
                try
                {
                    foreach (string dir in Directory.GetDirectories(blenderToolsDir, "open-fitter-core*", SearchOption.TopDirectoryOnly))
                    {
                        TryDeleteDirectory(dir, "OpenFitter Core");
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError($"[OpenFitter] Failed to delete OpenFitter Core directories: {e.Message}");
                }
            }

            string coreZip = Path.Combine(blenderToolsDir, OpenFitterConstants.CoreZipFileName);
            TryDeleteFile(coreZip, "OpenFitter Core zip");
        }

        public static void DeleteAddonFiles()
        {
            string blenderToolsDir = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            string addonsDir = Path.Combine(blenderToolsDir, "addons");
            string addonInstallPath = Path.Combine(addonsDir, OpenFitterConstants.RobustWeightTransferAddonName);
            TryDeleteDirectory(addonInstallPath, "Addon install");

            string tempRoot = Path.Combine(Path.GetTempPath(), "OpenFitter", "addons");
            string extractedAddonPath = Path.Combine(tempRoot, OpenFitterConstants.RobustWeightTransferAddonName);
            string addonZipPath = Path.Combine(tempRoot, OpenFitterConstants.RobustWeightTransferAddonZipFileName);
            TryDeleteDirectory(extractedAddonPath, "Addon temp");
            TryDeleteFile(addonZipPath, "Addon zip");
        }

        private static void TryDeleteDirectory(string path, string label)
        {
            if (!Directory.Exists(path)) return;

            try
            {
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[OpenFitter] Failed to delete {label}: {e.Message}");
            }
        }

        private static void TryDeleteFile(string path, string label)
        {
            if (!File.Exists(path)) return;

            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"[OpenFitter] Failed to delete {label}: {e.Message}");
            }
        }
    }
}
