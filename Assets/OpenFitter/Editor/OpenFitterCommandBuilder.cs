using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Builds command-line arguments for executing OpenFitter Core in Blender.
    /// </summary>
    public sealed class OpenFitterCommandBuilder
    {
        /// <summary>
        /// Builds the complete command-line arguments string for Blender.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="args"/> is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if required arguments are missing.</exception>
        public string BuildArguments(OpenFitterCoreArguments args)
        {
            if (!HasRequiredArgs(args))
            {
                throw new InvalidOperationException("Required arguments for OpenFitter Core are missing.");
            }

            // Run the core script directly (addon is pre-enabled during installation)
            string scriptToRun = GetAbsolutePath(args.ScriptPath);

            // Use target-typed new and cleaner collection initializer
            List<string> segments = new()
            {
                "--background",
                "--python",
                OpenFitterPathUtility.Quote(scriptToRun),
                "--",
                "--input",
                // GetAbsolutePath will now throw if null/empty, guaranteeing valid paths
                OpenFitterPathUtility.Quote(GetAbsolutePath(args.InputFbx)),
                "--output",
                OpenFitterPathUtility.Quote(GetAbsolutePath(args.OutputFbx)),
                "--base"
            };

            // Handle baseBlend: if provided, resolve to absolute; otherwise strict validation might catch it later if it ends up empty where not allowed.
            // Since HasRequiredArgs checks baseBlend, we assume it's valid here.
            string baseBlendPath = args.BaseBlend;
            if (!string.IsNullOrEmpty(baseBlendPath) && !Path.IsPathRooted(baseBlendPath))
            {
                baseBlendPath = Path.GetFullPath(baseBlendPath);
            }
            segments.Add(OpenFitterPathUtility.Quote(baseBlendPath));

            segments.Add("--base-fbx");
            segments.Add(OpenFitterPathUtility.Quote(GetAbsolutePaths(args.BaseFbxList)));

            segments.Add("--config");
            segments.Add(OpenFitterPathUtility.Quote(GetAbsolutePaths(args.ConfigList)));

            segments.Add("--init-pose");
            segments.Add(OpenFitterPathUtility.Quote(GetAbsolutePath(args.InitPose)));

            OpenFitterPathUtility.AppendIfValue(segments, "--hips-position", args.HipsPosition);

            var (blendShapesArg, blendShapeValuesArg) = GetBlendShapeArgs(args);

            // Null coalescing for lists using empty list default
            OpenFitterPathUtility.AppendIfValue(segments, "--blend-shapes", blendShapesArg);
            OpenFitterPathUtility.AppendIfValue(segments, "--blend-shape-values", blendShapeValuesArg);
            OpenFitterPathUtility.AppendIfValue(segments, "--blend-shape-mappings", string.Join(";", args.BlendShapeMappings ?? new()));
            OpenFitterPathUtility.AppendIfValue(segments, "--target-meshes", string.Join(";", args.TargetMeshes ?? new()));
            OpenFitterPathUtility.AppendIfValue(segments, "--mesh-renderers", string.Join(";", args.MeshRenderers ?? new()));
            OpenFitterPathUtility.AppendIfValue(segments, "--name-conv", string.Join(";", args.NameConv ?? new()));

            if (args.PreserveBoneNames)
            {
                segments.Add("--preserve-bone-names");
            }

            if (!args.Subdivide)
            {
                segments.Add("--no-subdivision");
            }

            if (!args.Triangulate)
            {
                segments.Add("--no-triangle");
            }

            return string.Join(" ", segments);
        }

        /// <summary>
        /// Checks if all required arguments are provided.
        /// </summary>
        public static bool HasRequiredArgs(OpenFitterCoreArguments args)
        {
            return !string.IsNullOrWhiteSpace(args.InputFbx)
                   && !string.IsNullOrWhiteSpace(args.BaseBlend)
                   && args.BaseFbxList is { Count: > 0 }
                   && args.ConfigList is { Count: > 0 }
                   && !string.IsNullOrWhiteSpace(args.InitPose);
        }

        private static string GetAbsolutePath(string? path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            return Path.IsPathRooted(path) ? path! : Path.GetFullPath(path!);
        }

        private static string GetAbsolutePaths(List<string> paths)
        {
            if (paths.Count == 0)
            {
                throw new ArgumentException("Path list cannot be null or empty.", nameof(paths));
            }

            return string.Join(";", paths.Select(GetAbsolutePath));
        }

        private static (string blendShapes, string blendShapeValues) GetBlendShapeArgs(OpenFitterCoreArguments args)
        {
            var entries = args.BlendShapeEntries;
            var enabledNames = new List<string>();
            var enabledValues = new List<string>();

            foreach (var entry in entries)
            {
                if (entry.enabled && !string.IsNullOrWhiteSpace(entry.customName))
                {
                    enabledNames.Add(entry.customName);
                    enabledValues.Add(entry.value.ToString("F1"));
                }
            }

            return (string.Join(";", enabledNames), string.Join(";", enabledValues));
        }
    }
}
