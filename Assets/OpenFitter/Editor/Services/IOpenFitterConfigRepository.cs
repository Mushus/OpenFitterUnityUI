using System.Collections.Generic;

namespace OpenFitter.Editor
{
    public interface IOpenFitterConfigRepository
    {
        List<ConfigInfo> RefreshAvailableConfigs();
        List<JsonAssetEntry> RefreshJsonAssets();
        ConfigInfo? GetConfigByPath(string configPath);
        List<BlendShapeSettingData> GetAvatarBlendShapes(string avatarDataPath);
    }
}
