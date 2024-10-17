using HarmonyLib;
using ProjectM.UI;
using ModernCamera.Utilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class HUDMenuPatch
{
    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnEnable))]
    [HarmonyPostfix]
    static void OnEnablePostfix() => CameraStateUtilities.IsMenuOpen = true;

    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnDisable))]
    [HarmonyPostfix]
    static void OnDisablePostfix() => CameraStateUtilities.IsMenuOpen = false;
}
