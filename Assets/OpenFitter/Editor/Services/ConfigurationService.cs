#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using OpenFitter.Editor;

namespace OpenFitter.Editor.Services
{
    public class ConfigurationService
    {
        private readonly OpenFitterConfigRepository configRepository;

        private const string TemplateAvatarName = "Template";

        // Caching available configs
        public List<ConfigInfo> AvailableConfigs { get; private set; } = new List<ConfigInfo>();
        public List<JsonAssetEntry> JsonAssets { get; private set; } = new List<JsonAssetEntry>();

        public List<ConfigInfo> CurrentSourceConfigs { get; private set; } = new List<ConfigInfo>();
        public List<ConfigInfo> CurrentTargetConfigs { get; private set; } = new List<ConfigInfo>();

        public List<string> SourceConfigNames { get; private set; } = new List<string>();
        public List<string> TargetConfigNames { get; private set; } = new List<string>();

        public int GetSourceConfigIndex(string? path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            int idx = CurrentSourceConfigs.FindIndex(c => c.configPath == path);
            return idx >= 0 ? idx + 1 : 0;
        }

        public int GetTargetConfigIndex(string? path)
        {
            if (string.IsNullOrEmpty(path)) return 0;
            int idx = CurrentTargetConfigs.FindIndex(c => c.configPath == path);
            return idx >= 0 ? idx + 1 : 0;
        }

        public ConfigurationService()
        {
            configRepository = new OpenFitterConfigRepository();
        }

        // Expose repository for FittingService if needed, or pass it in constructor
        public OpenFitterConfigRepository GetRepository() => configRepository;

        public List<ConfigInfo> RefreshAvailableConfigs()
        {
            AvailableConfigs = configRepository.RefreshAvailableConfigs();
            RefreshFittingOptions();
            return AvailableConfigs;
        }

        private void RefreshFittingOptions()
        {
            CurrentSourceConfigs = AvailableConfigs
                .Where(c => IsTemplateAvatarName(c.baseAvatar.name))
                .ToList();

            CurrentTargetConfigs = AvailableConfigs
                .Where(c => IsTemplateAvatarName(c.clothingAvatar.name))
                .ToList();

            SourceConfigNames.Clear();
            SourceConfigNames.Add("None");
            SourceConfigNames.AddRange(CurrentSourceConfigs.Select(c => c.clothingAvatar.name));

            TargetConfigNames.Clear();
            TargetConfigNames.Add("None");
            TargetConfigNames.AddRange(CurrentTargetConfigs.Select(c => c.baseAvatar.name));
        }

        private static bool IsTemplateAvatarName(string avatarName)
        {
            return avatarName.Equals(TemplateAvatarName, StringComparison.OrdinalIgnoreCase);
        }

        public List<JsonAssetEntry> RefreshJsonAssets()
        {
            return configRepository.RefreshJsonAssets();
        }

        public string GenerateOutputFbxPath(ConfigInfo config)
        {
            return OpenFitterPathUtility.GenerateOutputFbxPath(config);
        }

        public ConfigInfo? GetConfigByPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            return configRepository.GetConfigByPath(path);
        }

        public List<BlendShapeEntry> LoadBlendShapes(ConfigInfo sourceConfig)
        {
            var blendShapeEntries = new List<BlendShapeEntry>();

            // 1. Get from config's sourceBlendShapeSettings (Actual used BS)
            var usedBS = new HashSet<string>();

            foreach (var bs in sourceConfig.sourceBlendShapeSettings)
            {
                string displayName = !string.IsNullOrEmpty(bs.name) ? bs.name : bs.label;
                if (string.IsNullOrEmpty(displayName)) continue;

                blendShapeEntries.Add(new BlendShapeEntry
                {
                    enabled = true,
                    originalName = displayName,
                    customName = displayName,
                    value = bs.value
                });
                usedBS.Add(displayName);
            }

            // 2. Get from clothingAvatarDataPath (Visible BS list)
            if (!string.IsNullOrEmpty(sourceConfig.clothingAvatar.avatarDataPath))
            {
                var allBS = configRepository.GetAvatarBlendShapes(sourceConfig.clothingAvatar.avatarDataPath);
                foreach (var bs in allBS)
                {
                    string displayName = !string.IsNullOrEmpty(bs.label) ? bs.label : bs.name;
                    if (string.IsNullOrEmpty(displayName)) continue;

                    if (!usedBS.Contains(displayName))
                    {
                        float val = bs.value;
                        if (val < 0.001f) val = 100.0f; // Default to 100% if not specified

                        blendShapeEntries.Add(new BlendShapeEntry
                        {
                            enabled = false,
                            originalName = displayName,
                            customName = displayName,
                            value = val
                        });
                        usedBS.Add(displayName);
                    }
                }
            }

            return blendShapeEntries;
        }
    }
}
