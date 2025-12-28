#nullable enable
using System;
using System.Collections.Generic;

namespace OpenFitter.Editor
{
    /// <summary>
    /// Represents a blend shape configuration entry.
    /// </summary>
    [Serializable]
    public sealed class BlendShapeEntry
    {
        public bool enabled = true;
        public string originalName = "";
        public string customName = "";
        public float value = 100f;
    }

    /// <summary>
    /// Represents a blend shape setting from config JSON.
    /// </summary>
    [Serializable]
    public sealed class BlendShapeSettingData
    {
        public string name = "";
        public string label = "";
        public float value;
    }

    /// <summary>
    /// Contains information about an avatar (base or clothing).
    /// </summary>
    [Serializable]
    public sealed class AvatarDataInfo
    {
        public string name = "";
        public string defaultFBXPath = "";
        public string jsonPath = "";
        public string avatarDataPath = "";
    }

    /// <summary>
    /// Represents a complete configuration including base and clothing avatars.
    /// </summary>
    [Serializable]
    public sealed class ConfigInfo
    {
        public string configPath = "";
        public string displayName = "";
        public AvatarDataInfo baseAvatar = new();
        public AvatarDataInfo clothingAvatar = new();
        public string poseDataPath = "";
        public string initPosePath = "";
        public List<BlendShapeSettingData> sourceBlendShapeSettings = new();
        public List<BlendShapeSettingData> targetBlendShapeSettings = new();
        public List<BlendShapeSettingData> blendShapeFields = new();
    }



    /// <summary>
    /// Pure runtime arguments for OpenFitter Core CLI.
    /// </summary>
    public sealed class OpenFitterCoreArguments
    {
        public string ScriptPath { get; set; } = string.Empty;
        public string InputFbx { get; set; } = string.Empty;
        public string OutputFbx { get; set; } = string.Empty;
        public string BaseBlend { get; set; } = OpenFitterConstants.DefaultBaseBlendPath;
        public List<string> BaseFbxList { get; set; } = new();
        public List<string> ConfigList { get; set; } = new();
        public string InitPose { get; set; } = string.Empty;
        public string HipsPosition { get; set; } = string.Empty;

        public string BlendShapes { get; set; } = string.Empty;
        public string BlendShapeValues { get; set; } = string.Empty;
        public List<BlendShapeEntry> BlendShapeEntries { get; set; } = new();
        public List<string> BlendShapeMappings { get; set; } = new();
        public List<string> TargetMeshes { get; set; } = new();
        public List<string> MeshRenderers { get; set; } = new();
        public List<string> NameConv { get; set; } = new();

        public bool PreserveBoneNames { get; set; }
        public bool Subdivide { get; set; }
        public bool Triangulate { get; set; }

        /// <summary>
        /// Creates a new instance from OpenFitterState and ConfigInfo.
        /// </summary>
        public static OpenFitterCoreArguments FromState(Services.OpenFitterState state, ConfigInfo config, string scriptPath)
        {
            return new OpenFitterCoreArguments
            {
                ScriptPath = scriptPath,
                InputFbx = OpenFitterPathUtility.ResolveFbxPath(!string.IsNullOrEmpty(state.InputFbx) ? state.InputFbx : config.clothingAvatar.defaultFBXPath),
                OutputFbx = "output.fbx", // Expected to be overwritten by strategy
                BaseBlend = OpenFitterConstants.DefaultBaseBlendPath,
                BaseFbxList = !string.IsNullOrEmpty(config.baseAvatar.defaultFBXPath)
                    ? new List<string> { OpenFitterPathUtility.ResolveFbxPath(config.baseAvatar.defaultFBXPath) }
                    : new List<string>(),
                ConfigList = new List<string> { config.configPath },
                InitPose = config.baseAvatar.jsonPath,
                BlendShapes = state.BlendShapes,
                BlendShapeValues = state.BlendShapeValues,
                BlendShapeMappings = new List<string>(state.BlendShapeMappings),
                TargetMeshes = new List<string>(state.TargetMeshes),
                MeshRenderers = new List<string>(state.MeshRenderers),
                NameConv = new List<string>(state.NameConv),
                PreserveBoneNames = state.PreserveBoneNames,
                Subdivide = state.Subdivide,
                Triangulate = state.Triangulate
            };
        }
    }

    /// <summary>
    /// Represents a JSON asset found in the project.
    /// </summary>
    [Serializable]
    public sealed class JsonAssetEntry
    {
        public string assetPath = "";
        public string fullPath = "";
        public string displayName = "";
    }

    /// <summary>
    /// Internal structure for parsing config JSON files.
    /// </summary>
    [Serializable]
    internal sealed class ConfigData
    {
        public string poseDataPath = "";
        public string fieldDataPath = "";
        public string baseAvatarDataPath = "";
        public string clothingAvatarDataPath = "";
        public BlendShapeSettingData[] sourceBlendShapeSettings = Array.Empty<BlendShapeSettingData>();
        public BlendShapeSettingData[] targetBlendShapeSettings = Array.Empty<BlendShapeSettingData>();
        public BlendShapeSettingData[] blendShapeFields = Array.Empty<BlendShapeSettingData>();
    }

    /// <summary>
    /// Internal structure for parsing avatar data JSON files.
    /// </summary>
    [Serializable]
    internal sealed class AvatarData
    {
        public string name = "";
        public string defaultFBXPath = "";
        public string basePose = "";
        public BlendShapeSettingData[] blendShapeFields = Array.Empty<BlendShapeSettingData>();
    }
}

