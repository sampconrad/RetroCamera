using HarmonyLib;
using RetroCamera.Configuration;
using ProjectM.UI;
using StunShared.UI;
using UnityEngine.InputSystem;
using RetroCamera.Utilities;
using static RetroCamera.Configuration.KeybindCategories.KeybindCategory;

namespace RetroCamera.Patches;

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

        foreach (var category in KeybindsManager._keybindCategories.Values)
        {
            __instance.AddHeader(category.NameKey);
            Keybind keybind = KeybindsManager._keybindsByName.TryGetValue(category.Name, out keybind) ? keybind : null;

            if (keybind == null)
            {
                continue;
            }

            var binding = UIHelper.InstantiatePrefabUnderAnchor(__instance.ControlsInputEntryPrefab, __instance.ContentNode);
            binding.Initialize(
                ProjectM.ControllerType.KeyboardAndMouse,
                keybind.InputFlag,
                ProjectM.AnalogInputAction.None,
                true,
                onClick: (Il2CppSystem.Action<SettingsEntry_Binding, bool, ProjectM.ButtonInputAction, ProjectM.AnalogInputAction, bool>)__instance.OnEntryButtonClicked,
                onClear: (Il2CppSystem.Action<SettingsEntry_Binding, ProjectM.ButtonInputAction>)__instance.OnEntryCleared
            );

            binding.SetInputInfo(category.NameKey, category.NameKey);
            binding.SetPrimary(keybind.PrimaryName);
            binding.SetSecondary(keybind.SecondaryName);

            var keybindEntry = binding as SettingsEntryBase;
            __instance.EntriesSelectionGroup.AddEntry(ref keybindEntry);

            /*
            foreach (var keybinding in category.InputActionMap.)
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
            */
        }
    }

    [HarmonyPatch(typeof(RebindingMenu), nameof(RebindingMenu.OnClick_ResetButton))]
    [HarmonyPrefix]
    static void ResetButtonPrefix()
    {
        foreach (var category in KeybindsManager._keybindCategories.Values)
        {
            foreach (InputAction action in category.InputActionMap.actions)
            {
                action.RemoveAllBindingOverrides();
            }
        }

        Persistence.SaveKeybinds();
    }
}

