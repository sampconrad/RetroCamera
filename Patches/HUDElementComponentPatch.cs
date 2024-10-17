using HarmonyLib;
using ProjectM.UI;
using UnityEngine;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class HUDElementComponentPatch
{
    [HarmonyPatch(typeof(HUDElementComponent), nameof(HUDElementComponent.UpdateVisibility))]
    [HarmonyPostfix]
    static void UpdateVisibilityPostfix(HUDElementComponent __instance)
    {
        if (__instance.gameObject.name.Equals("InteractorEntry(Clone)"))
        {
            foreach (CanvasGroup canvasGroup in __instance.GetComponentsInChildren<CanvasGroup>())
            {
                canvasGroup.alpha = 1f;
            }
        }
    }
}
