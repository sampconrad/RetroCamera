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
        if (WheelVisible)
        {
            if (__instance._CurrentActiveWheel != null && !__instance._CurrentActiveWheel.IsVisible())
            {
                CameraState.IsMenuOpen = false;
                WheelVisible = false;
            }
            else if (__instance._CurrentActiveWheel == null)
            {
                CameraState.IsMenuOpen = false;
                WheelVisible = false;
            }
        }
        else if (__instance._CurrentActiveWheel != null && __instance._CurrentActiveWheel.IsVisible())
        {
            WheelVisible = true;
            CameraState.IsMenuOpen = true;
        }
    }
}
