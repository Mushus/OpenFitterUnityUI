using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using OpenFitter;
using OpenFitter.Editor.Services;

namespace OpenFitter.Editor.Downloaders
{
    /// <summary>
    /// Manages Blender addon download with progress tracking for a single package.
    /// Responsibility: Download and extract addon only. Installation is handled separately.
    /// </summary>
    public sealed class BlenderAddonDownloader : ISetupTask
    {
        private readonly string addonUrl;
        private readonly string addonName;
        private readonly string zipFileName;
        private UnityWebRequest? downloadRequest;
        private string downloadTargetPath = string.Empty;
        private string extractedAddonPath = string.Empty;

        /// <summary>
        /// Create a new BlenderAddonDownloader for a specific addon package.
        /// </summary>
        /// <param name="addonUrl">The URL to download the addon from</param>
        /// <param name="addonName">The name of the addon</param>
        /// <param name="zipFileName">The name of the zip file to download</param>
        public BlenderAddonDownloader(string addonUrl, string addonName, string zipFileName)
        {
            this.addonUrl = addonUrl;
            this.addonName = addonName;
            this.zipFileName = zipFileName;
            InitializePaths();
        }

        private void InitializePaths()
        {
            string root = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            downloadTargetPath = Path.Combine(root, zipFileName);
            extractedAddonPath = Path.Combine(root, "addons", addonName);
        }

        public bool IsRunning => downloadRequest != null;

        public bool IsReady => File.Exists(downloadTargetPath);

        public float Progress => downloadRequest != null ? Mathf.Clamp01(downloadRequest.downloadProgress) : 0f;

        public string ZipPath => downloadTargetPath;

        // Legacy Aliases
        public float DownloadProgress => Progress;
        public bool IsDownloading => IsRunning;

        public SetupResult Start()
        {
            if (downloadRequest != null)
            {
                return SetupResult.Failed(I18n.Tr("Download is in progress. Please wait for it to finish."));
            }

            string? addonsDir = Path.GetDirectoryName(downloadTargetPath);
            if (addonsDir != null && !Directory.Exists(addonsDir))
            {
                Directory.CreateDirectory(addonsDir);
            }

            try
            {
                downloadRequest = new UnityWebRequest(addonUrl, UnityWebRequest.kHttpVerbGET)
                {
                    downloadHandler = new DownloadHandlerFile(downloadTargetPath)
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
                CleanupDownloadRequest();
                return SetupResult.Failed(I18n.Tr("Addon download failed. ") + error);
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

        /// <summary>
        /// Gets the path to the extracted addon.
        /// Returns empty string if not yet extracted.
        /// </summary>
        public string GetExtractedAddonPath()
        {
            return extractedAddonPath ?? string.Empty;
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

        private void CleanupDownloadRequest()
        {
            if (downloadRequest != null)
            {
                downloadRequest.Dispose();
                downloadRequest = null;
            }
        }

        // Legacy method support
        public SetupResult StartDownload() => Start();
        public SetupResult UpdateDownloadProgress() => Update();
        public void AbortDownload() => Abort();
    }
}
