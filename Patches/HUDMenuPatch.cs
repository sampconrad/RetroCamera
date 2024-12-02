using HarmonyLib;
using ProjectM.UI;
using RetroCamera.Utilities;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class HUDMenuPatch
{
    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnEnable))]
    [HarmonyPostfix]
    static void OnEnablePostfix() => CameraState.IsMenuOpen = true;

    [HarmonyPatch(typeof(HUDMenu), nameof(HUDMenu.OnDisable))]
    [HarmonyPostfix]
    static void OnDisablePostfix() => CameraState.IsMenuOpen = false;
}
