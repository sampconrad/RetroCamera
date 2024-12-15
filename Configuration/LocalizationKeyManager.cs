using Stunlock.Core;
using Stunlock.Localization;
using Guid = Il2CppSystem.Guid;

namespace RetroCamera.Configuration;
internal static class LocalizationKeyManager
{
    static readonly Dictionary<AssetGuid, string> AssetGuids = [];
    public static LocalizationKey CreateKey(string value)
    {
        LocalizationKey localizationKey = new(AssetGuid.FromGuid(Guid.NewGuid()));

        if (!HasKey(localizationKey))
        {
            AssetGuids.TryAdd(localizationKey.GetGuid(), value);
        }

        return localizationKey;
    }
    public static bool HasKey(LocalizationKey key)
    {
        return AssetGuids.ContainsKey(key.GetGuid());
    }

    // Currently unused methods for localizationKeys and assetGuids
    /*
    public static Nullable_Unboxed<LocalizationKey> CreateNullableKey(string value)
    {
        return new Nullable_Unboxed<LocalizationKey>(CreateKey(value));
    }

    public static string GetKey(AssetGuid guid)
    {
        return AssetGuids[guid];
    }
    public static string GetKey(LocalizationKey key)
    {
        return GetKey(key.GetGuid());
    }
    public static bool HasKey(AssetGuid guid)
    {
        return AssetGuids.ContainsKey(guid);
    }
    */
}
