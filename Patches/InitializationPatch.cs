using HarmonyLib;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class InitializationPatch
{
    [HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(GameDataManager __instance)
    {
        try
        {
            if (__instance.GameDataInitialized && !Core.HasInitialized)
            {
                Core.Initialize(__instance);

                if (Core.HasInitialized)
                {
                    Core.Log.LogInfo($"|{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] initialized on client!");
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log.LogError($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] failed to initialize on client, exiting on try-catch: {ex}");
        }
    }
}
