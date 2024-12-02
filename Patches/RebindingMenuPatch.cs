using HarmonyLib;
using RetroCamera.Configuration;
using ProjectM.UI;
using StunShared.UI;
using UnityEngine.InputSystem;

namespace RetroCamera.Patches;

/*
[HarmonyPatch]
internal static class RebindingMenuPatch
{
    [HarmonyPatch(typeof(RebindingMenu), nameof(RebindingMenu.Start))]
    [HarmonyPostfix]
    static void StartPostfix(RebindingMenu __instance)
    {
        if (__instance._BindingTypeToDisplay != ProjectM.ControllerType.KeyboardAndMouse)
        {
            return;
        }

        foreach (var category in KeybindsManager.KeybindingCategories.Values)
        {
            __instance.AddHeader(category.NameKey);
            foreach (var keybinding in category.KeybindingMap.Values)
            {
                var binding = UIHelper.InstantiatePrefabUnderAnchor(__instance.ControlsInputEntryPrefab, __instance.ContentNode);
                binding.Initialize(
                    ProjectM.ControllerType.KeyboardAndMouse,
                    keybinding.InputFlag,
                    ProjectM.AnalogInputAction.None,
                    true,
                    onClick: (Il2CppSystem.Action<SettingsEntry_Binding, bool, ProjectM.ButtonInputAction, ProjectM.AnalogInputAction, bool>)__instance.OnEntryButtonClicked,
                    onClear: (Il2CppSystem.Action<SettingsEntry_Binding, ProjectM.ButtonInputAction>)__instance.OnEntryCleared
                );

                binding.SetInputInfo(keybinding.NameKey, keybinding.NameKey);
                binding.SetPrimary(keybinding.PrimaryName);
                binding.SetSecondary(keybinding.SecondaryName);

                var keybindEntry = binding as SettingsEntryBase;
                __instance.EntriesSelectionGroup.AddEntry(ref keybindEntry);
            }
        }
    }

    [HarmonyPatch(typeof(RebindingMenu), nameof(RebindingMenu.OnClick_ResetButton))]
    [HarmonyPrefix]
    static void ResetButtonPrefix()
    {
        foreach (KeybindingCategory category in KeybindsManager.KeybindingCategories.Values)
        {
            foreach (InputAction action in category.InputActionMap.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }

        KeybindsManager.sav();
    }
}
*/
