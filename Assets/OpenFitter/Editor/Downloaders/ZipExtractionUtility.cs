#nullable enable
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace OpenFitter.Editor.Downloaders
{
    /// <summary>
    /// Utility for extracting zip files with common folder handling.
    /// </summary>
    public static class ZipExtractionUtility
    {
        /// <summary>
        /// Extracts a zip file to a final directory, handling wrapped folder structures.
        /// GitHub zips typically contain a single wrapper folder.
        /// </summary>
        /// <param name="zipPath">Path to the zip file to extract</param>
        /// <param name="finalTargetPath">Final destination directory path</param>
        public static void ExtractZipToDirectory(string zipPath, string finalTargetPath)
        {
            if (string.IsNullOrEmpty(zipPath)) throw new ArgumentNullException(nameof(zipPath));
            if (string.IsNullOrEmpty(finalTargetPath)) throw new ArgumentNullException(nameof(finalTargetPath));

            if (!File.Exists(zipPath))
            {
                throw new FileNotFoundException("Zip file not found: " + zipPath);
            }

            string tempExtractPath = Path.Combine(Path.GetTempPath(), "OpenFitter_" + Path.GetRandomFileName());

            try
            {
                // Clean up old temp if exists
                if (Directory.Exists(tempExtractPath))
                {
                    Directory.Delete(tempExtractPath, true);
                }

                // Clean up final target if exists
                if (Directory.Exists(finalTargetPath))
                {
                    Directory.Delete(finalTargetPath, true);
                }

                Directory.CreateDirectory(tempExtractPath);

                // Extract ZIP to temp location
                ZipFile.ExtractToDirectory(zipPath, tempExtractPath);

                // Find the inner folder (GitHub zips have a wrapper folder like "repo-main")
                string[] extractedDirs = Directory.GetDirectories(tempExtractPath);

                // Strict check: GitHub zips usually have exactly one wrapper. 
                // If multiple or zero, we fallback to tempExtractPath itself.
                string sourceDir = extractedDirs.Length == 1 ? extractedDirs[0] : tempExtractPath;

                // Ensure parent directory of target exists
                string? parentDir = Path.GetDirectoryName(finalTargetPath);
                if (!string.IsNullOrEmpty(parentDir) && !Directory.Exists(parentDir))
                {
                    Directory.CreateDirectory(parentDir!);
                }

                // Move extracted content to final location
                // If sourceDir == tempExtractPath, we can't Move top level if finalTargetPath is nested? 
                // Actually Directory.Move works if paths are normalized.
                Directory.Move(sourceDir, finalTargetPath);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[OpenFitter] Extraction failed: {ex.Message}");
                throw; // Rethrow to let caller handle
            }
            finally
            {
                // Clean up temp directory
                if (Directory.Exists(tempExtractPath))
                {
                    try
                    {
                        Directory.Delete(tempExtractPath, true);
                    }
                    catch (Exception cleanupEx)
                    {
                        UnityEngine.Debug.LogWarning($"[OpenFitter] Temp cleanup failed: {cleanupEx.Message}");
                    }
                }
            }
        }
    }
}

