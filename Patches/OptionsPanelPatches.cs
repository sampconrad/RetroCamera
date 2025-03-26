using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using ProjectM.UI;
using RetroCamera.Configuration;
using RetroCamera.Utilities;
using StunShared.UI;
using TMPro;
using UnityEngine;
using static RetroCamera.Configuration.OptionActions;
using static RetroCamera.Configuration.OptionCategories;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class OptionsPanelPatches
{
    [HarmonyPatch(typeof(OptionsPanel_Interface), nameof(OptionsPanel_Interface.Start))]
    [HarmonyPostfix]
    static void Start(OptionsPanel_Interface __instance)
    {
        foreach (OptionCategory optionCategory in OptionsManager._optionCategories.Values)
        {
            if (optionCategory.Options.Count == 0)
            {
                continue;
            }

            __instance.AddHeader(optionCategory.LocalizationKey);

            foreach (string id in optionCategory.Options)
            {
                if (OptionCategory.TryGetToggle(id, out ToggleOption toggleOption))
                {
                    SettingsEntry_Checkbox settingsEntryCheckbox = UIHelper.InstantiatePrefabUnderAnchor(__instance.CheckboxPrefab, __instance.ContentNode);

                    settingsEntryCheckbox.Initialize(
                        toggleOption.NameKey,
                        new(toggleOption.DescKey),
                        toggleOption.DefaultValue,
                        toggleOption.Value,
                        OnChange(toggleOption)
                    );
                    
                    /*
                    settingsEntryCheckbox.HeaderText.LocalizationKey = toggleOption.NameKey;
                    settingsEntryCheckbox.Description = new(toggleOption.DescKey);
                    settingsEntryCheckbox._DefaultValue = toggleOption.DefaultValue;
                    settingsEntryCheckbox._InitValue = toggleOption.Value;
                    settingsEntryCheckbox.OnChangeAction = OnChange(toggleOption);
                    settingsEntryCheckbox.enabled = true;
                    */

                    SettingsEntryBase settingsEntryBase = settingsEntryCheckbox;
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                }
                else if (OptionCategory.TryGetSlider(id, out SliderOption sliderOption))
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
                    
                    /*
                    settingsEntrySlider.HeaderText.LocalizationKey = sliderOption.NameKey;
                    settingsEntrySlider.Description = new(sliderOption.DescKey);
                    settingsEntrySlider.Slider.minValue = sliderOption.MinValue;
                    settingsEntrySlider.Slider.maxValue = sliderOption.MaxValue;
                    settingsEntrySlider._DefaultValue = sliderOption.DefaultValue;
                    settingsEntrySlider._InitValue = sliderOption.Value;
                    settingsEntrySlider._Decimals = sliderOption.Decimals;
                    settingsEntrySlider.Slider.wholeNumbers = sliderOption.Decimals == 0;
                    settingsEntrySlider.OnChangeAction = OnChange(sliderOption);
                    settingsEntrySlider.Slider.SetStepSize(sliderOption.StepValue);
                    settingsEntrySlider.enabled = true;
                    */

                    SettingsEntryBase settingsEntryBase = settingsEntrySlider;
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase, true);
                }
                else if (OptionCategory.TryGetDropdown(id, out DropdownOption dropdownOption))
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
                    
                    /*
                    settingsEntryDropdown.HeaderText.LocalizationKey = dropdownOption.NameKey;
                    settingsEntryDropdown.Description = new(dropdownOption.DescKey);
                    settingsEntryDropdown.SetOptions(options, dropdownOption.Value);
                    settingsEntryDropdown._DefaultValue = dropdownOption.DefaultValue;
                    settingsEntryDropdown.OnChangeAction = OnChange(dropdownOption);
                    settingsEntryDropdown.enabled = true;
                    */

                    SettingsEntryBase settingsEntryBase = settingsEntryDropdown;
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                }
                else if (OptionCategory.TryGetDivider(id, out string dividerText))
                {
                    GameObject dividerObject = CreateDivider(__instance.ContentNode, dividerText);

                    /*
                    int dividerIndex = GetDividerIndexBasedOnName(dividerText);

                    if (dividerIndex == -1)
                    {
                        Core.Log.LogWarning($"Could not find index for divider {dividerText}");
                        continue;
                    }
                    else __instance.EntriesSelectionGroup.AddChildren(dividerObject.transform, ref dividerIndex);
                    */
                }
            }
        }
    }
    

    [HarmonyPatch(typeof(Options_ControlsPanel), nameof(Options_ControlsPanel.Start))]
    [HarmonyPostfix]
    static void StartPostfix(OptionsPanel_Interface __instance)
    {
        foreach (OptionCategory optionCategory in OptionsManager._optionCategories.Values)
        {
            if (optionCategory.Options.Count == 0)
            {
                continue;
            }

            foreach (string id in optionCategory.Options)
            {
                if (OptionCategory.TryGetSlider(id, out SliderOption sliderOption))
                {
                    __instance.AddSlider(sliderOption.NameKey, sliderOption.DescKey, sliderOption.MinValue, sliderOption.MaxValue, sliderOption.DefaultValue,
                        sliderOption.Value, sliderOption.Decimals, sliderOption.Decimals == 0, OnChange(sliderOption));
                }
            }
        }
    }
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
        textMeshDivider.font = textMeshArray[0].font;
        textMeshDivider.fontSize = 20f;
        textMeshDivider.SetText(dividerText);

        dividerGameObject.SetActive(true);
        return dividerGameObject;
    }
    static Il2CppSystem.Action<T> OnChange<T>(OptionAction<T> option)
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
