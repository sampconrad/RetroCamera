using HarmonyLib;
using ProjectM.UI;
using ModernCamera.Utilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class ActionWheelSystemPatch
{
    public static bool WheelVisible = false;

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(ActionWheelSystem __instance)
    {
        if (__instance == null) return;
        else if (WheelVisible)
        {
            if (__instance._CurrentActiveWheel != null && !__instance._CurrentActiveWheel.IsVisible())
            {
                CameraStateUtilities.IsMenuOpen = false;
                WheelVisible = false;
            }
            else if (__instance._CurrentActiveWheel == null)
            {
                CameraStateUtilities.IsMenuOpen = false;
                WheelVisible = false;
            }
        }
        else if (__instance._CurrentActiveWheel != null && __instance._CurrentActiveWheel.IsVisible())
        {
            WheelVisible = true;
            CameraStateUtilities.IsMenuOpen = true;
        }
    }
}
