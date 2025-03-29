using Stunlock.Core;
using Stunlock.Localization;
using System.Security.Cryptography;
using System.Text;
using Guid = Il2CppSystem.Guid;

namespace RetroCamera.Configuration;
internal static class LocalizationManager
{
    public const string HEADER = MyPluginInfo.PLUGIN_NAME;
    public static LocalizationKey _sectionHeader;

    static readonly Dictionary<AssetGuid, string> _assetGuids = [];
    public static IReadOnlyDictionary<AssetGuid, string> AssetGuids => _assetGuids;
    public static void LocalizeText()
    {
        _sectionHeader = GetLocalizationKey(HEADER);

        foreach (var keyValuePair in AssetGuids)
        {
            AssetGuid assetGuid = keyValuePair.Key;
            string localizedString = keyValuePair.Value;

            Localization._LocalizedStrings.TryAdd(assetGuid, localizedString);
        }
    }
    static AssetGuid GetAssetGuid(string text)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));

        Guid uniqueGuid = new(hashBytes[..16]);
        return AssetGuid.FromGuid(uniqueGuid);
    }
    public static LocalizationKey GetLocalizationKey(string value)
    {
        AssetGuid assetGuid = GetAssetGuid(value);
        _assetGuids.TryAdd(assetGuid, value);

        return new(assetGuid);
    }
}
