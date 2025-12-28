using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using OpenFitter;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor.Downloaders
{
    /// <summary>
    /// Manages OpenFitter Core download and script discovery.
    /// </summary>
    public sealed class OpenFitterCoreDownloader : ISetupTask
    {
        private UnityWebRequest? downloadRequest;
        private string downloadTargetPath = string.Empty;

        public bool IsRunning => downloadRequest != null;

        public float Progress => downloadRequest != null ? Mathf.Clamp01(downloadRequest.downloadProgress) : 0f;

        private bool IsCoreInstalled => !string.IsNullOrEmpty(TryResolveScriptPath());
        private bool IsZipDownloaded => File.Exists(Path.Combine(GetCoreRootParent(), OpenFitterConstants.CoreZipFileName));

        public bool IsReady => IsZipDownloaded || IsCoreInstalled;

        // Legacy accessors
        public float DownloadProgress => Progress;
        public bool IsDownloading => IsRunning;

        /// <summary>
        /// Starts the download process.
        /// </summary>
        public SetupResult Start()
        {
            if (downloadRequest != null)
            {
                return SetupResult.Failed(I18n.Tr("Download is in progress. Please wait for it to finish."));
            }

            string root = GetCoreRootParent();
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            string zipPath = Path.Combine(root, OpenFitterConstants.CoreZipFileName);
            downloadTargetPath = zipPath;

            try
            {
                downloadRequest = new UnityWebRequest(OpenFitterConstants.CoreZipUrl, UnityWebRequest.kHttpVerbGET)
                {
                    downloadHandler = new DownloadHandlerFile(zipPath)
                    {
                        removeFileOnAbort = true
                    }
                };
                downloadRequest.SendWebRequest();
                return SetupResult.Success();
            }
            catch (Exception ex)
            {
                downloadRequest?.Dispose();
                downloadRequest = null;
                return SetupResult.Failed(I18n.Tr("Failed to start: ") + ex.Message);
            }
        }

        public SetupResult Update()
        {
            if (downloadRequest == null)
            {
                return SetupResult.Failed(I18n.Tr("Download not started."));
            }

            float progress = Mathf.Clamp01(downloadRequest.downloadProgress);

            if (!downloadRequest.isDone)
            {
                return SetupResult.InProgress(Path.GetFileName(downloadTargetPath ?? ""));
            }

            if (downloadRequest.result != UnityWebRequest.Result.Success)
            {
                string error = downloadRequest.error ?? I18n.Tr("Unknown error");
                downloadRequest.Dispose();
                downloadRequest = null;
                return SetupResult.Failed(I18n.Tr("Core download failed. ") + error);
            }

            try
            {
                downloadRequest.Dispose();
                downloadRequest = null;
                return SetupResult.Success();
            }
            catch (Exception ex)
            {
                downloadRequest?.Dispose();
                downloadRequest = null;
                return SetupResult.Failed(I18n.Tr("Finalizing download failed: ") + ex.Message);
            }
        }

        public void Abort()
        {
            if (downloadRequest != null)
            {
                downloadRequest.Abort();
                downloadRequest.Dispose();
                downloadRequest = null;
            }
        }

        // Backward compatibility
        public SetupResult StartDownload() => Start();
        public SetupResult UpdateDownloadProgress() => Update();
        public void AbortDownload() => Abort();

        public string TryResolveScriptPath()
        {
            string root = GetCoreRootParent();
            string coreRoot = ResolveCoreRoot(root);
            if (string.IsNullOrEmpty(coreRoot))
            {
                return string.Empty;
            }

            return FindCoreScriptPath(coreRoot);
        }

        private static string GetCoreRootParent()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
        }

        private static string GetExpectedCoreFolder()
        {
            return Path.Combine(GetCoreRootParent(), OpenFitterConstants.CoreFolderName);
        }

        private static string ResolveCoreRoot(string root)
        {
            if (string.IsNullOrEmpty(root) || !Directory.Exists(root))
            {
                return string.Empty;
            }

            string expected = GetExpectedCoreFolder();
            if (Directory.Exists(expected))
            {
                return expected;
            }

            var candidates = Directory.GetDirectories(root, "open-fitter-core*", SearchOption.TopDirectoryOnly)
                .OrderByDescending(Directory.GetLastWriteTimeUtc)
                .ToArray();

            return candidates.Length > 0 ? candidates[0] : string.Empty;
        }

        private static string FindCoreScriptPath(string coreRoot)
        {
            if (string.IsNullOrEmpty(coreRoot) || !Directory.Exists(coreRoot))
            {
                return string.Empty;
            }

            string[] preferredNames =
            {
                "retarget_script2_10.py",
                "retarget_script2.py",
                "retarget_script.py",
                "retarget.py"
            };

            foreach (string name in preferredNames)
            {
                string[] matches = Directory.GetFiles(coreRoot, name, SearchOption.AllDirectories);
                if (matches.Length > 0)
                {
                    return matches[0];
                }
            }

            string devPath = Path.Combine(coreRoot, "dev");
            if (Directory.Exists(devPath))
            {
                string[] devMatches = Directory.GetFiles(devPath, "*.py", SearchOption.TopDirectoryOnly);
                if (devMatches.Length > 0)
                {
                    return devMatches[0];
                }
            }

            return string.Empty;
        }
    }
}
