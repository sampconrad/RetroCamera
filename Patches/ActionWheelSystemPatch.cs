using HarmonyLib;
using ProjectM.UI;
using RetroCamera.Utilities;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class ActionWheelSystemPatch
{
    public static bool _wheelVisible = false;

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(ActionWheelSystem __instance)
    {
        if (_wheelVisible)
        {
            if (__instance._CurrentActiveWheel != null && !__instance._CurrentActiveWheel.IsVisible())
            {
                CameraState.IsMenuOpen = false;
                _wheelVisible = false;
            }
            else if (__instance._CurrentActiveWheel == null)
            {
                CameraState.IsMenuOpen = false;
                _wheelVisible = false;
            }
        }
        else if (__instance._CurrentActiveWheel != null && __instance._CurrentActiveWheel.IsVisible())
        {
            _wheelVisible = true;
            CameraState.IsMenuOpen = true;
        }
    }
}
