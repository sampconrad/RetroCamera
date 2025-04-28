using HarmonyLib;
using ProjectM;
using ProjectM.UI;
using RetroCamera.Utilities;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class InitializationPatches
{
    [HarmonyPatch(typeof(GameDataManager), nameof(GameDataManager.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(GameDataManager __instance)
    {
        try
        {
            if (__instance.GameDataInitialized && !Core._initialized)
            {
                Core.Initialize(__instance);

                if (Core._initialized)
                {
                    Core.Log.LogInfo($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] initialized on client!");
                }
            }
        }
        catch (Exception ex)
        {
            Core.Log.LogError($"{MyPluginInfo.PLUGIN_NAME}[{MyPluginInfo.PLUGIN_VERSION}] failed to initialize on client, exiting on try-catch: {ex}");
        }
    }

    [HarmonyPatch(typeof(ClientBootstrapSystem), nameof(ClientBootstrapSystem.OnDestroy))]
    [HarmonyPrefix]
    static void OnDestroyPrefix(ClientBootstrapSystem __instance)
    {
        Core.ResetStates();
    }

    /*
    static bool _isFound = false;

    [HarmonyPatch(typeof(DayNightCycleMoodSystem), nameof(DayNightCycleMoodSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix(DayNightCycleMoodSystem __instance)
    {
        if (!Core._initialized) return;
        else if (_isFound) return;

        NativeArray<Entity> entities = __instance.EntityQueries[0].ToEntityArray(Allocator.Temp);

        try
        {
            foreach (Entity entity in entities)
            {
                if (entity.Has<DayNightCycle>())
                {
                    ClearSkies._dayNightCycleSingleton = entity;
                    ClearSkies.Initialize();

                    Core.LogEntity(Core._client, entity);
                    _isFound = true;

                    break;
                }
            }
        }
        finally
        {
            entities.Dispose();
        }
    }
    */

    [HarmonyPatch(typeof(UICanvasSystem), nameof(UICanvasSystem.UpdateHideIfDisabled))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(UICanvasBase canvas)
    {
        if (!Core._initialized) return;
        else if (ClearSkies._initialized) return;

        ClearSkies.Initialize();
    }
}
