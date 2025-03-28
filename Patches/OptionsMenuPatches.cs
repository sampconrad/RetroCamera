using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ProjectM;
using ProjectM.UI;
using RetroCamera.Configuration;
using RetroCamera.Utilities;
using Stunlock.Core;
using Stunlock.Localization;
using StunShared.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static RetroCamera.Configuration.Keybinding.Keybinding;
using static RetroCamera.Configuration.SettingEvents;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class OptionsMenuPatches
{
    static readonly AssetGuid _assetGuid = AssetGuid.FromString("eb117dbf-0c9b-4403-ad73-dea763f8f2f6");
    static bool _shouldLocalize = true;

    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.Start))]
    [HarmonyPostfix]
    static void StartPostfix() => CameraState.IsMenuOpen = true;

    [HarmonyPatch(typeof(OptionsMenu_Base), nameof(OptionsMenu_Base.OnDestroy))]
    [HarmonyPostfix]
    static void OnDestroyPostfix() => CameraState.IsMenuOpen = false;

    [HarmonyPatch(typeof(OptionsPanel_Interface), nameof(OptionsPanel_Interface.Start))]
    [HarmonyPostfix]
    static void StartPostfix(OptionsPanel_Interface __instance)
    {
        if (_shouldLocalize)
        {
            if (!Localization.Initialized)
            {
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization isn't ready!");
            }
            else
            {
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization ready, adding keys!");

                foreach (var keyValuePair in LocalizationManager.AssetGuids)
                {
                    AssetGuid assetGuid = keyValuePair.Key;
                    string value = keyValuePair.Value;

                    Localization._LocalizedStrings.TryAdd(assetGuid, value);
                }

                _shouldLocalize = false;
                // _optionsReady = true;
            }
        }

        foreach (OptionSettings optionCategory in OptionManager.OptionGroupSettingsByName.Values)
        {
            if (optionCategory.SettingNames.Count == 0)
            {
                continue;
            }

            __instance.AddHeader(new(_assetGuid));

            foreach (string id in optionCategory.SettingNames)
            {
                try
                {
                    if (OptionSettings.TryGetToggle(id, out Toggle toggleOption))
                    {
                        SettingsEntry_Checkbox settingsEntryCheckbox = UIHelper.InstantiatePrefabUnderAnchor(__instance.CheckboxPrefab, __instance.ContentNode);

                        settingsEntryCheckbox.Initialize(
                            toggleOption.NameKey,
                            new(toggleOption.DescKey),
                            toggleOption.DefaultValue,
                            toggleOption.Value,
                            OnChange(toggleOption)
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntryCheckbox;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                    }
                    else if (OptionSettings.TryGetSlider(id, out Slider sliderOption))
                    {
                        SettingsEntry_Slider settingsEntrySlider = UIHelper.InstantiatePrefabUnderAnchor(__instance.SliderPrefab, __instance.ContentNode);

                        settingsEntrySlider.Initialize(
                            sliderOption.NameKey,
                            new(sliderOption.DescKey),
                            sliderOption.MinValue,
                            sliderOption.MaxValue,
                            sliderOption.DefaultValue,
                            sliderOption.Value,
                            sliderOption.Decimals,
                            sliderOption.Decimals == 0,
                            OnChange(sliderOption),
                            fixedStepValue: sliderOption.StepValue
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntrySlider;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                    }
                    else if (OptionSettings.TryGetDropdown(id, out Dropdown dropdownOption))
                    {
                        Il2CppSystem.Collections.Generic.List<string> options = new(dropdownOption.Values.Count);
                        foreach (string option in dropdownOption.Values)
                        {
                            options.Add(option);
                        }

                        SettingsEntry_Dropdown settingsEntryDropdown = UIHelper.InstantiatePrefabUnderAnchor(__instance.DropdownPrefab, __instance.ContentNode);

                        settingsEntryDropdown.Initialize(
                            dropdownOption.NameKey,
                            new(dropdownOption.DescKey),
                            new([]),
                            options,
                            dropdownOption.DefaultValue,
                            dropdownOption.Value,
                            OnChange(dropdownOption)
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntryDropdown;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                    }
                    else if (OptionSettings.TryGetDivider(id, out string dividerText))
                    {
                        GameObject dividerObject = CreateDivider(__instance.ContentNode, dividerText);


                    }
                }
                catch (Exception ex)
                {
                    Core.Log.LogError($"Failed to create option {id}: {ex.Message}");
                }
            }
        }
    }

    [HarmonyPatch(typeof(RebindingMenu), nameof(RebindingMenu.Start))]
    [HarmonyPostfix]
    static void StartPostfix(RebindingMenu __instance)
    {
        if (__instance._BindingTypeToDisplay != ControllerType.KeyboardAndMouse)
        {
            return;
        }

        foreach (var category in KeybindManager.KeybindCategoriesByName.Values)
        {
            __instance.AddHeader(category.NameKey);

            if (!KeybindManager.KeybindsByKeybindCategory.TryGetValue(category, out var keybinds))
            {
                Core.Log.LogWarning($"No keybinds found for category! ({category.NameKey})");
                continue;
            }

            foreach (var keybind in keybinds)
            {
                SettingsEntry_Binding settingsEntryBinding = UIHelper.InstantiatePrefabUnderAnchor(__instance.ControlsInputEntryPrefab, __instance.ContentNode);

                settingsEntryBinding.Initialize(
                    ControllerType.KeyboardAndMouse,
                    keybind.InputFlag,
                    AnalogInputAction.None,
                    true,
                    false,
                    true,
                    onClick: (Il2CppSystem.Action<SettingsEntry_Binding, bool, ButtonInputAction, AnalogInputAction, bool>)__instance.OnEntryButtonClicked,
                    onClear: (Il2CppSystem.Action<SettingsEntry_Binding, ButtonInputAction>)__instance.OnEntryCleared,
                    true
                );

                settingsEntryBinding.SetInputInfo(keybind.NameKey, keybind.DescriptionKey);
                settingsEntryBinding.SetPrimary(keybind.PrimaryName);
                settingsEntryBinding.SetSecondary(keybind.SecondaryName);

                settingsEntryBinding.PrimaryButton.onClick.AddListener((UnityAction)(() => RefreshKeybind(__instance, settingsEntryBinding, keybind).Start()));
                settingsEntryBinding.SecondaryButton.gameObject.SetActive(false);

                SettingsEntryBase settingsEntryBase = settingsEntryBinding;
                __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
            }
        }
    }

    const float REBIND_TIMEOUT = 5f;
    static IEnumerator RefreshKeybind(RebindingMenu __instance, SettingsEntry_Binding binding, Keybind keybind)
    {
        KeyCode newKey = KeyCode.None;
        __instance.OnEntryButtonClicked(binding, true, ButtonInputAction.None, AnalogInputAction.None, true);

        while (true)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    if (key == KeyCode.Escape)
                    {
                        Core.Log.LogWarning($"[{binding.name}] Rebind cancelled");
                        __instance.OnRebindingComplete(false);

                        yield break;
                    }

                    /*
                    if ((now - startTime).TotalSeconds > REBIND_TIMEOUT)
                    {
                        Core.Log.LogWarning($"Rebind timeout");
                        yield break;
                    }
                    */

                    newKey = key;
                    break;
                }
            }

            if (newKey != KeyCode.None)
                break;

            yield return null;
        }

        KeybindManager.Rebind(keybind, newKey);
        binding.SetPrimary(keybind.PrimaryName);
        __instance.OnRebindingCancel(false, false);
        // __instance.OnEntryCleared(binding, ButtonInputAction.None);

        Core.Log.LogWarning($"[{binding.name}] rebound to {newKey}");
    }

    /*
    static IEnumerator RefreshKeybind(SettingsEntry_Binding binding, ButtonInputAction buttonInputAction)
    {
        if (!_settingsEntryKeybinds.TryGetValue(binding, out var keybind))
        {
            Core.Log.LogWarning($"No Keybind found for SettingsEntry_Binding!");
            return;
        }

        KeyCode keyCode = default;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                keyCode = key;
                break; // only register the first one
            }
        }

        KeybindManager.Rebind(keybind, keyCode);
    }
    */

    /*
    [HarmonyPatch(typeof(OptionsPanel_Interface), nameof(OptionsPanel_Interface.Start))]
    [HarmonyPostfix]
    static void StartPostfix(OptionsPanel_Interface __instance)
    {
        if (_shouldLocalize)
        {
            if (!Localization.Initialized)
            {
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization isn't ready!");
            }
            else
            {
                Core.Log.LogWarning("[OptionsPanel_Interface.Start()] Localization ready, adding keys!");

                foreach (var keyValuePair in LocalizationKeyManager.AssetGuids)
                {
                    AssetGuid assetGuid = keyValuePair.Key;
                    string value = keyValuePair.Value;

                    Localization._LocalizedStrings.TryAdd(assetGuid, value);
                }

                _shouldLocalize = false;
                // _optionsReady = true;
            }
        }

        foreach (OptionGroups.Settings optionCategory in OptionsManager.OptionGroupSettingsByName.Values)
        {
            if (optionCategory.SettingNames.Count == 0)
            {
                continue;
            }

            __instance.AddHeader(new(_assetGuid));

            foreach (string id in optionCategory.SettingNames)
            {
                try
                {
                    if (OptionGroups.Settings.TryGetToggle(id, out Toggle toggleOption))
                    {
                        SettingsEntry_Checkbox settingsEntryCheckbox = UIHelper.InstantiatePrefabUnderAnchor(__instance.CheckboxPrefab, __instance.ContentNode);

                        settingsEntryCheckbox.Initialize(
                            toggleOption.NameKey,
                            new(toggleOption.DescKey),
                            toggleOption.DefaultValue,
                            toggleOption.Value,
                            OnChange(toggleOption)
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntryCheckbox;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                    }
                    else if (OptionGroups.Settings.TryGetSlider(id, out Slider sliderOption))
                    {
                        SettingsEntry_Slider settingsEntrySlider = UIHelper.InstantiatePrefabUnderAnchor(__instance.SliderPrefab, __instance.ContentNode);

                        settingsEntrySlider.Initialize(
                            sliderOption.NameKey,
                            new(sliderOption.DescKey),
                            sliderOption.MinValue,
                            sliderOption.MaxValue,
                            sliderOption.DefaultValue,
                            sliderOption.Value,
                            sliderOption.Decimals,
                            sliderOption.Decimals == 0,
                            OnChange(sliderOption),
                            fixedStepValue: sliderOption.StepValue
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntrySlider;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                    }
                    else if (OptionGroups.Settings.TryGetDropdown(id, out Dropdown dropdownOption))
                    {
                        Il2CppSystem.Collections.Generic.List<string> options = new(dropdownOption.Values.Count);
                        foreach (string option in dropdownOption.Values)
                        {
                            options.Add(option);
                        }

                        SettingsEntry_Dropdown settingsEntryDropdown = UIHelper.InstantiatePrefabUnderAnchor(__instance.DropdownPrefab, __instance.ContentNode);

                        settingsEntryDropdown.Initialize(
                            dropdownOption.NameKey,
                            new(dropdownOption.DescKey),
                            new([]),
                            options,
                            dropdownOption.DefaultValue,
                            dropdownOption.Value,
                            OnChange(dropdownOption)
                        );

                        SettingsEntryBase settingsEntryBase = settingsEntryDropdown;
                        __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                    }
                    else if (OptionGroups.Settings.TryGetDivider(id, out string dividerText))
                    {
                        GameObject dividerObject = CreateDivider(__instance.ContentNode, dividerText);


                    }
                }
                catch (Exception ex)
                {
                    Core.Log.LogError($"Failed to create option {id}: {ex.Message}");
                }
            }
        }
    }
    */

    /*  
    int dividerIndex = GetDividerIndexBasedOnName(dividerText);

    if (dividerIndex == -1)
    {
        Core.Log.LogWarning($"Could not find index for divider {dividerText}");
        continue;
    }
    else __instance.EntriesSelectionGroup.AddChildren(dividerObject.transform, ref dividerIndex);

    [HarmonyPatch(typeof(Options_ControlsPanel), nameof(Options_ControlsPanel.Start))]
    [HarmonyPostfix]
    static void StartPostfix(Options_ControlsPanel __instance)
    {
        foreach (var category in KeybindsManager.KeybindCategoriesByName.Values)
        {
            __instance.AddHeader(category.NameKey);

            if (!KeybindsManager.KeybindsByKeybindCategory.TryGetValue(category, out var keybinds))
            {
                Core.Log.LogWarning($"No keybinds found for category: {category.NameKey}");
                continue;
            }

            foreach (var keybinding in keybinds)
            {
                SettingsEntry_Binding settingsEntryBinding = UIHelper.InstantiatePrefabUnderAnchor(__instance.ControlsInputEntryPrefab, __instance.ContentNode);

                settingsEntryBinding.Initialize(
                    ControllerType.KeyboardAndMouse,
                    keybinding.InputFlag,
                    AnalogInputAction.None,
                    true,
                    false,
                    true,
                    onClick: (Il2CppSystem.Action<SettingsEntry_Binding, bool, ButtonInputAction, AnalogInputAction, bool>)__instance.OnEntryButtonClicked,
                    onClear: (Il2CppSystem.Action<SettingsEntry_Binding, ButtonInputAction>)RefreshKeybind
                );

                __instance._Entries.Add(settingsEntryBinding);
                _settingsEntryKeybinds.TryAdd(settingsEntryBinding, keybinding);
            }
        }
    }

    static readonly Dictionary<SettingsEntry_Binding, Keybind> _settingsEntryKeybinds = [];

    [HarmonyPatch(typeof(Options_ControlsPanel), nameof(Options_ControlsPanel.Start))]
    [HarmonyPrefix]
    static void StartPrefix(Options_ControlsPanel __instance)
    {
        Core.Log.LogWarning("Options_ControlsPanel.Start()");
    }

    static void RefreshKeybind(SettingsEntry_Binding binding, ButtonInputAction buttonInputAction)
    {
        if (!_settingsEntryKeybinds.TryGetValue(binding, out var keybind))
        {
            Core.Log.LogWarning($"No Keybind found for SettingsEntry_Binding!");
            return;
        }

        KeyCode keyCode = default;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(key))
            {
                keyCode = key;
                break; // only register the first one
            }
        }

        KeybindsManager.Rebind(keybind, keyCode);
    }
    */
    static GameObject CreateDivider(Transform parent, string dividerText)
    {
        GameObject dividerGameObject = new("Divider");

        RectTransform dividerTransform = dividerGameObject.AddComponent<RectTransform>();
        dividerTransform.SetParent(parent);
        dividerTransform.localScale = Vector3.one;
        dividerTransform.sizeDelta = new(0f, 28f);

        UnityEngine.UI.Image dividerImage = dividerGameObject.AddComponent<UnityEngine.UI.Image>();
        dividerImage.color = new(0.12f, 0.152f, 0.2f, 0.15f);

        UnityEngine.UI.LayoutElement dividerLayout = dividerGameObject.AddComponent<UnityEngine.UI.LayoutElement>();
        dividerLayout.preferredHeight = 28f;

        GameObject dividerTextGameObject = new("Text");
        RectTransform dividerTextTransform = dividerTextGameObject.AddComponent<RectTransform>();
        dividerTextTransform.SetParent(dividerGameObject.transform);
        dividerTextTransform.localScale = Vector3.one;

        TextMeshProUGUI textMeshDivider = dividerTextGameObject.AddComponent<TextMeshProUGUI>();
        Il2CppArrayBase<TextMeshProUGUI> textMeshArray = parent.GetComponentsInChildren<TextMeshProUGUI>();
        TMP_FontAsset fontAsset = textMeshArray.First().font;

        textMeshDivider.alignment = TextAlignmentOptions.Center;
        textMeshDivider.fontStyle = FontStyles.SmallCaps;
        textMeshDivider.font = fontAsset;
        textMeshDivider.fontSize = 20f;
        textMeshDivider.SetText(dividerText);

        dividerGameObject.SetActive(true);
        return dividerGameObject;
    }
    static Il2CppSystem.Action<T> OnChange<T>(ChangeSetting<T> option)
    {
        // Core.Log.LogInfo($"Option {option.Name} {option.Value}...");

        return (Il2CppSystem.Action<T>)(value =>
        {
            option.SetValue(value);
            Persistence.SaveOptions();
        });
    }
    static int GetDividerIndexBasedOnName(string dividerText)
    {
        // Example logic for determining index based on divider name
        if (dividerText == "ThirdPersonAiming")
            return 0; // Place at the start of the section
        else if (dividerText == "ThirdPersonZoom")
            return 3; // Place after a few entries
        else if (dividerText == "ThirdPersonPitch")
            return 7; // Place further down
        else if (dividerText == "OverShoulder")
            return 11; // Place towards the end of the section
        else
            return -1;
    }
}
