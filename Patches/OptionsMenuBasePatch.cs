using HarmonyLib;
using ProjectM.UI;
using ModernCamera.Utilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class OptionsMenuBasePatch
{
    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.Start))]
    [HarmonyPostfix]
    static void StartPostfix() => CameraState.IsMenuOpen = true;

    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.OnDestroy))]
    [HarmonyPostfix]
    static void OnDestroyPostfix() => CameraState.IsMenuOpen = false;
}
