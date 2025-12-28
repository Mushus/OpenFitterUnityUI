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
    /// Manages Blender download and auto-detection functionality.
    /// </summary>
    public sealed class BlenderDownloader : ISetupTask
    {
        private UnityWebRequest? downloadRequest;
        private string downloadTargetPath = string.Empty;

        public bool IsRunning => downloadRequest != null;
        public float Progress => downloadRequest != null ? Mathf.Clamp01(downloadRequest.downloadProgress) : 0f;

        // Helper to check blender existence without args reference if needed, 
        // but since we want to be consistent with OpenFitterModel:
        private bool IsBlenderInstalled => !string.IsNullOrEmpty(EditorPrefs.GetString("OpenFitterWindow.blenderPath", string.Empty)) && File.Exists(EditorPrefs.GetString("OpenFitterWindow.blenderPath", string.Empty));

        private bool IsZipDownloaded => File.Exists(Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder, OpenFitterConstants.BlenderZipFileName));

        public bool IsReady => IsZipDownloaded || IsBlenderInstalled;

        public SetupResult Start()
        {
            if (downloadRequest != null)
            {
                return SetupResult.Failed(I18n.Tr("Download is in progress. Please wait for it to finish."));
            }

            string blenderDownloadUrl = BuildBlenderDownloadUrl(OpenFitterConstants.RecommendedBlenderVersion);
            string blenderDownloadFolder = Path.Combine(Directory.GetCurrentDirectory(), OpenFitterConstants.BlenderToolsFolder);

            if (string.IsNullOrWhiteSpace(blenderDownloadFolder))
            {
                return SetupResult.Failed(I18n.Tr("Download folder is invalid."));
            }
            if (!Directory.Exists(blenderDownloadFolder))
            {
                Directory.CreateDirectory(blenderDownloadFolder);
            }

            string targetPath = BuildDownloadTargetPath(blenderDownloadFolder);
            try
            {
                downloadRequest = new UnityWebRequest(blenderDownloadUrl, UnityWebRequest.kHttpVerbGET)
                {
                    downloadHandler = new DownloadHandlerFile(targetPath)
                    {
                        removeFileOnAbort = true
                    }
                };
                downloadTargetPath = targetPath;
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
                return SetupResult.Failed(I18n.Tr("Blender download failed. ") + error);
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
                return SetupResult.Failed(I18n.Tr("Finalizing download failed. ") + ex.Message);
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

        // Backward compatibility shims
        public SetupResult StartDownload() => Start();
        public SetupResult UpdateDownloadProgress() => Update();
        public void AbortDownload() => Abort();

        /// <summary>
        /// Builds the Blender download URL based on version.
        /// </summary>
        public static string BuildBlenderDownloadUrl(string version)
        {
            string releaseFolder = BuildReleaseFolder(version);
            string fileName = BuildDownloadFileName(version);
            return $"https://download.blender.org/release/{releaseFolder}/{fileName}";
        }

        private static string BuildReleaseFolder(string version)
        {
            string[] parts = version.Split('.');
            if (parts.Length >= 2)
            {
                return $"Blender{parts[0]}.{parts[1]}";
            }

            return $"Blender{version}";
        }

        private static string BuildDownloadFileName(string version)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return $"blender-{version}-windows-x64.zip";
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                return $"blender-{version}-macos-x64.dmg";
            }

            return $"blender-{version}-linux-x64.tar.xz";
        }

        private static string BuildDownloadTargetPath(string folder)
        {
            return Path.Combine(folder, OpenFitterConstants.BlenderZipFileName);
        }

    }
}
