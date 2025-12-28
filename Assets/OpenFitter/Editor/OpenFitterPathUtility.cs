using System;
using System.Collections.Generic;
using System.IO;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Utility methods for file path operations.
    /// </summary>
    public static class OpenFitterPathUtility
    {
        /// <summary>
        /// Resolves a directory from a path string (handles semicolon-separated lists).
        /// </summary>
        public static string ResolveDirectory(string rawPath)
        {
            if (string.IsNullOrWhiteSpace(rawPath))
            {
                return Directory.GetCurrentDirectory();
            }

            string first = rawPath.Split(new[] { ';' }, 2)[0];

            string dir = Path.GetDirectoryName(first);
            return string.IsNullOrEmpty(dir) ? Directory.GetCurrentDirectory() : dir;
        }

        /// <summary>
        /// Quotes a string value for command-line usage, escaping internal quotes.
        /// Handles Windows-specific escaping for trailing backslashes.
        /// </summary>
        public static string Quote(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "\"\"";
            }

            // Standard Windows command line escaping:
            // 1. Double internal quotes: " -> \"
            // 2. If the string ends with a backslash, double it to avoid escaping the closing quote: \ -> \\
            string escaped = value!.Replace("\"", "\\\"");
            if (escaped.EndsWith("\\"))
            {
                escaped += "\\";
            }

            return $"\"{escaped}\"";
        }

        /// <summary>
        /// Appends a flag and quoted value to a command segments list if the value is not empty.
        /// </summary>
        public static void AppendIfValue(ICollection<string> segments, string flag, string? value)
        {
            if (segments is null) throw new ArgumentNullException(nameof(segments));

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            segments.Add(flag);
            segments.Add(Quote(value));
        }

        /// <summary>
        /// Converts an absolute path to a Unity project-relative path (starting with Assets/).
        /// </summary>
        public static string ToRelativePath(string? absolutePath)
        {
            if (string.IsNullOrEmpty(absolutePath)) return string.Empty;

            try
            {
                // Normalize both paths to use forward slashes and absolute paths
                string projectRoot = Path.GetFullPath(Directory.GetCurrentDirectory()).Replace('\\', '/').TrimEnd('/');
                string normalizedPath = Path.GetFullPath(absolutePath!).Replace('\\', '/');

                if (normalizedPath.StartsWith(projectRoot, StringComparison.OrdinalIgnoreCase))
                {
                    string relative = normalizedPath[projectRoot.Length..].TrimStart('/');
                    return relative;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"[OpenFitter] Failed to resolve relative path for '{absolutePath}': {ex.Message}");
            }

            return absolutePath!;
        }
        public static string GenerateOutputFbxPath(ConfigInfo config)
        {
            string projectPath = Directory.GetCurrentDirectory();
            string outputDir = Path.Combine(projectPath, "Assets", "OpenFitter", "Outputs");

            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
                UnityEngine.Debug.Log(string.Format(I18n.Tr("Created output directory: {0}"), outputDir));
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filename = $"{config.clothingAvatar.name}_to_{config.baseAvatar.name}_{timestamp}.fbx";

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, '_');
            }

            return Path.Combine(outputDir, filename);
        }

        public static string ResolveFbxPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;
            if (path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase)) return path;

            // Load asset to resolve its internal meshes. 
            // LoadAssetAtPath needs a relative path.
            string relPath = ToRelativePath(path);
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.GameObject>(relPath);
            if (asset == null) return path;

            return ResolveFbxPath(asset);
        }

        public static string ResolveFbxPath(UnityEngine.GameObject gameObject)
        {
            if (gameObject == null) return string.Empty;

            // If it's already an FBX asset (Model), this will return the FBX path
            string path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            if (!string.IsNullOrEmpty(path) && path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            // If it's a prefab or scene object, try to find a mesh from it
            // We search for SkinnedMeshRenderer first as it's common for avatars
            var smr = gameObject.GetComponentInChildren<UnityEngine.SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null)
            {
                string meshPath = UnityEditor.AssetDatabase.GetAssetPath(smr.sharedMesh);
                if (!string.IsNullOrEmpty(meshPath) && meshPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                {
                    return meshPath;
                }
            }

            var meshFilter = gameObject.GetComponentInChildren<UnityEngine.MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                string meshPath = UnityEditor.AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
                if (!string.IsNullOrEmpty(meshPath) && meshPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
                {
                    return meshPath;
                }
            }

            // Fallback to PrefabUtility for variant/instance handling
            path = UnityEditor.PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
            if (string.IsNullOrEmpty(path))
            {
                path = UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            }
            return path;
        }
    }
}

