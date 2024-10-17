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
        Core.Log.LogInfo("EscapeMenuView.OnEnable");

        if (!IsEscapeMenuOpen)
        {
            IsEscapeMenuOpen = true;
            StateUtilities.IsMenuOpen = true;
        }
    }
    static void Disable()
    {
        Core.Log.LogInfo("EscapeMenuView.OnDisable");

        if (IsEscapeMenuOpen)
        {
            IsEscapeMenuOpen = false;
            StateUtilities.IsMenuOpen = false;
        }
    }
}
