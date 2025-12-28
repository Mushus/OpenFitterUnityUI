using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OpenFitter.Editor
{
    public sealed class OpenFitterConfigRepository : IOpenFitterConfigRepository
    {
        public List<ConfigInfo> RefreshAvailableConfigs()
        {
            var availableConfigs = new List<ConfigInfo>();

            string[] configGuids = AssetDatabase.FindAssets("config_ t:TextAsset", new[] { "Assets" });

            foreach (string guid in configGuids)
            {
                string configAssetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!configAssetPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string configFullPath = Path.GetFullPath(configAssetPath);

                try
                {
                    ConfigInfo? configInfo = ParseConfigFile(configFullPath);
                    if (configInfo != null)
                    {
                        availableConfigs.Add(configInfo);
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogWarning($"Failed to parse config: {configAssetPath}\n{ex.Message}");
                }
            }

            UnityEngine.Debug.Log(string.Format(I18n.Tr("Found {0} valid config files"), availableConfigs.Count));
            return availableConfigs;
        }

        public List<JsonAssetEntry> RefreshJsonAssets()
        {
            var jsonAssets = new List<JsonAssetEntry>();
            string[] guids = AssetDatabase.FindAssets("t:TextAsset", new[] { "Assets" });

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!assetPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                jsonAssets.Add(new JsonAssetEntry
                {
                    assetPath = assetPath,
                    fullPath = Path.GetFullPath(assetPath),
                    displayName = assetPath
                });
            }

            return jsonAssets.OrderBy(entry => entry.displayName, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public ConfigInfo? GetConfigByPath(string configPath)
        {
            return ParseConfigFile(configPath);
        }

        public List<BlendShapeSettingData> GetAvatarBlendShapes(string avatarDataPath)
        {
            if (!File.Exists(avatarDataPath)) return new List<BlendShapeSettingData>();

            try
            {
                string json = File.ReadAllText(avatarDataPath);
                var data = JsonUtility.FromJson<AvatarData>(json);
                if (data != null && data.blendShapeFields != null)
                {
                    var blendShapes = new List<BlendShapeSettingData>(data.blendShapeFields);
                    // Convert all values from 0-1 to 0-100
                    foreach (var bs in blendShapes)
                    {
                        bs.value *= 100f;
                    }
                    return blendShapes;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to get avatar blend shapes: {avatarDataPath}\n{ex.Message}");
            }

            return new List<BlendShapeSettingData>();
        }

        private static ConfigInfo? ParseConfigFile(string configPath)
        {
            if (!File.Exists(configPath))
            {
                return null;
            }

            string configJson = File.ReadAllText(configPath);
            var configData = JsonUtility.FromJson<ConfigData>(configJson);

            if (configData == null)
            {
                return null;
            }

            string configDir = Path.GetDirectoryName(configPath);

            string baseAvatarDataPath = ResolvePath(configData.baseAvatarDataPath, configDir);
            string clothingAvatarDataPath = ResolvePath(configData.clothingAvatarDataPath, configDir);

            AvatarDataInfo? baseAvatar = ParseAvatarDataFile(baseAvatarDataPath);
            AvatarDataInfo? clothingAvatar = ParseAvatarDataFile(clothingAvatarDataPath);

            if (baseAvatar == null || clothingAvatar == null)
            {
                return null;
            }

            // Parse BlendShape settings from config
            var sourceBlendShapes = new List<BlendShapeSettingData>();
            foreach (var bs in configData.sourceBlendShapeSettings)
            {
                bs.value *= 100f; // Convert 0-1 to 0-100
            }
            sourceBlendShapes.AddRange(configData.sourceBlendShapeSettings);

            var targetBlendShapes = new List<BlendShapeSettingData>();
            foreach (var bs in configData.targetBlendShapeSettings)
            {
                bs.value *= 100f; // Convert 0-1 to 0-100
            }
            targetBlendShapes.AddRange(configData.targetBlendShapeSettings);

            // Convert blend shape field values from 0-1 to 0-100
            foreach (var bs in configData.blendShapeFields)
            {
                bs.value *= 100f; // Convert 0-1 to 0-100
            }

            return new ConfigInfo
            {
                configPath = configPath,
                displayName = clothingAvatar.name,
                baseAvatar = baseAvatar,
                clothingAvatar = clothingAvatar,
                poseDataPath = configData.poseDataPath,
                initPosePath = baseAvatar.jsonPath,
                sourceBlendShapeSettings = sourceBlendShapes,
                targetBlendShapeSettings = targetBlendShapes,
                blendShapeFields = configData.blendShapeFields != null ? new List<BlendShapeSettingData>(configData.blendShapeFields) : new List<BlendShapeSettingData>()
            };
        }

        private static AvatarDataInfo? ParseAvatarDataFile(string avatarDataPath)
        {
            if (!File.Exists(avatarDataPath))
            {
                return null;
            }

            try
            {
                string avatarJson = File.ReadAllText(avatarDataPath);
                var avatarData = JsonUtility.FromJson<AvatarData>(avatarJson);

                if (string.IsNullOrEmpty(avatarData.name))
                {
                    return null;
                }

                string avatarDir = Path.GetDirectoryName(avatarDataPath);

                string fbxPath = ResolvePath(avatarData.defaultFBXPath, avatarDir);
                string basePosePath = ResolvePath(avatarData.basePose, avatarDir);

                return new AvatarDataInfo
                {
                    name = avatarData.name,
                    defaultFBXPath = fbxPath,
                    jsonPath = basePosePath,
                    avatarDataPath = avatarDataPath
                };
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to parse avatar data: {avatarDataPath}\n{ex.Message}");
                return null;
            }
        }

        private static string ResolvePath(string rawPath, string fallbackDir)
        {
            if (string.IsNullOrEmpty(rawPath))
            {
                return rawPath;
            }

            if (Path.IsPathRooted(rawPath))
            {
                return rawPath;
            }

            if (rawPath.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase) ||
                rawPath.StartsWith("Assets\\", StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), rawPath));
            }

            return Path.GetFullPath(Path.Combine(fallbackDir, rawPath));
        }
    }
}
