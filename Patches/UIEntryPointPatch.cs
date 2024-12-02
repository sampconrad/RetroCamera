using HarmonyLib;
using ProjectM.UI;
using RetroCamera.Utilities;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class UIEntryPointPatch
{
    [HarmonyPatch(typeof(UIEntryPoint), nameof(UIEntryPoint.Awake))]
    [HarmonyPostfix]
    static void AwakePostfix()
    {
        CameraState.Reset();
    }
}
