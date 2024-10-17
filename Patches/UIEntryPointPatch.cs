using HarmonyLib;
using ProjectM.UI;
using ModernCamera.Utilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class UIEntryPointPatch
{
    [HarmonyPatch(typeof(UIEntryPoint), nameof(UIEntryPoint.Awake))]
    [HarmonyPostfix]
    static void AwakePostfix()
    {
        StateUtilities.Reset();
    }
}
