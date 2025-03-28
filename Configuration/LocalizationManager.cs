using Stunlock.Core;
using Stunlock.Localization;
using System.Security.Cryptography;
using System.Text;
using Guid = Il2CppSystem.Guid;

namespace RetroCamera.Configuration;
internal static class LocalizationManager
{
    public static readonly Dictionary<AssetGuid, string> AssetGuids = [];
    public static LocalizationKey CreateKey(string value)
    {
        // LocalizationKey localizationKey = new(AssetGuid.FromGuid(Guid.NewGuid()));
        // AssetGuid assetGuid = localizationKey.GetGuid();

        AssetGuid assetGuid = GetAssetGuid(value);
        AssetGuids.TryAdd(assetGuid, value);

        /*
        if (Localization.Initialized) Localization._LocalizedStrings.TryAdd(assetGuid, value);
        else
        {
            Core.Log.LogWarning("[LocalizationKeyManager] Localization isn't initialized yet!");
        }
        */

        return new(assetGuid);
    }
    static AssetGuid GetAssetGuid(string combinedHashString)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedHashString));

        Guid uniqueGuid = new(hashBytes[..16]);
        return AssetGuid.FromGuid(uniqueGuid);
    }
}
