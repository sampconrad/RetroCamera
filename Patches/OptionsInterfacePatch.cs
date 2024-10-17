using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using ModernCamera.Configuration;
using ModernCamera.Utilities;
using ProjectM.UI;
using Stunlock.Localization;
using StunShared.UI;
using TMPro;
using UnityEngine;
using static ModernCamera.Configuration.OptionActions;
using static ModernCamera.Configuration.OptionCategories;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class OptionsInterfacePatch
{
    [HarmonyPatch(typeof(OptionsPanel_Interface), nameof(OptionsPanel_Interface.Start))]
    [HarmonyPostfix]
    static void Start(OptionsPanel_Interface __instance)
    {
        foreach (OptionCategory optionCategory in OptionsManager.OptionCategories.Values)
        {
            if (optionCategory.Options.Count == 0)
            {
                continue;
            }

            __instance.AddHeader(optionCategory.LocalizationKey);

            foreach (string id in optionCategory.Options)
            {
                if (optionCategory.TryGetToggle(id, out ToggleOption toggleOption))
                {
                    LocalizedString localizedString = LocalizedString.Create(toggleOption.DescKey);
                    Il2CppSystem.Nullable<LocalizedString> desc = new(localizedString);

                    SettingsEntry_Checkbox settingsEntryCheckbox = UIHelper.InstantiatePrefabUnderAnchor(__instance.CheckboxPrefab, __instance.ContentNode);
                    settingsEntryCheckbox.Initialize(
                        toggleOption.NameKey,
                        desc,
                        toggleOption.DefaultValue,
                        toggleOption.Value,
                        OnChange(toggleOption)
                    );

                    SettingsEntryBase settingsEntryBase = settingsEntryCheckbox;
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                }
                else if (optionCategory.TryGetSlider(id, out SliderOption sliderOption))
                {
                    SettingsEntry_Slider settingsEntrySlider = UIHelper.InstantiatePrefabUnderAnchor(__instance.SliderPrefab, __instance.ContentNode);
                    settingsEntrySlider.Initialize(
                        sliderOption.NameKey,
                        new Nullable_Unboxed<LocalizationKey>(sliderOption.DescKey),
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
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                }
                else if (optionCategory.TryGetDropdown(id, out DropdownOption dropdownOption))
                {
                    LocalizedString localizedString = LocalizedString.Create(dropdownOption.DescKey);
                    Il2CppSystem.Nullable<LocalizedString> descKey = new(localizedString);

                    Il2CppSystem.Collections.Generic.List<string> options = new(dropdownOption.Values.Count);
                    foreach (string option in dropdownOption.Values)
                    {
                        options.Add(option);
                    }

                    SettingsEntry_Dropdown settingsEntryDropdown = UIHelper.InstantiatePrefabUnderAnchor(__instance.DropdownPrefab, __instance.ContentNode);
                    settingsEntryDropdown.Initialize(
                        dropdownOption.NameKey,
                        descKey,
                        options,
                        dropdownOption.DefaultValue,
                        dropdownOption.Value,
                        OnChange(dropdownOption),
                        false,
                        false,
                        false
                    );

                    SettingsEntryBase settingsEntryBase = settingsEntryDropdown;
                    __instance.EntriesSelectionGroup.AddEntry(ref settingsEntryBase);
                }
                else if (optionCategory.TryGetDivider(id, out string dividerText))
                {
                    GameObject dividerObject = CreateDivider(__instance.ContentNode, dividerText);
                    int dividerIndex = GetDividerIndexBasedOnName(dividerText);

                    if (dividerIndex == -1)
                    {
                        Core.Log.LogWarning($"Could not find index for divider {dividerText}");
                        continue;
                    }
                    else __instance.EntriesSelectionGroup.AddChildren(dividerObject.transform, ref dividerIndex);
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
        textMeshDivider.font = fontAsset;
        textMeshDivider.fontSize = 20f;
        textMeshDivider.SetText(dividerText);

        dividerGameObject.SetActive(true);
        return dividerGameObject;
    }
    static Il2CppSystem.Action<T> OnChange<T>(OptionAction<T> option)
    {
        Core.Log.LogInfo($"Option {option.Name} {option.Value}...");

        return (Il2CppSystem.Action<T>)(value =>
        {
            option.SetValue(value);
            PersistenceUtilities.SaveOptions();
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
