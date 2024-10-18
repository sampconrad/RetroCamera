using HarmonyLib;
using ProjectM.UI;
using ModernCamera.Utilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class EscapeMenuViewPatch
{
    public static bool IsEscapeMenuOpen = false;

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnEnable))]
    [HarmonyPostfix]
    static void OnEnablePostfix() => Enable();

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnDestroy))]
    [HarmonyPostfix]
    static void OnDestroyPostfix() => Disable();
    static void Enable()
    {
        if (!IsEscapeMenuOpen)
        {
            IsEscapeMenuOpen = true;
            CameraStateUtilities.IsMenuOpen = true;
        }
    }
    static void Disable()
    {
        if (IsEscapeMenuOpen)
        {
            IsEscapeMenuOpen = false;
            CameraStateUtilities.IsMenuOpen = false;
        }
    }
}
