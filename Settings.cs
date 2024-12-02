using RetroCamera.Configuration;
using RetroCamera.Patches;
using RetroCamera.Utilities;
using UnityEngine;
using static RetroCamera.Configuration.KeybindCategories;
using static RetroCamera.Configuration.KeybindCategories.KeybindCategory;
using static RetroCamera.Configuration.OptionCategories;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera;
internal static class Settings
{
    public static bool Enabled { get => EnabledOption.Value; set => EnabledOption.SetValue(value); }
    public static bool FirstPersonEnabled { get => FirstPersonEnabledOption.Value; set => FirstPersonEnabledOption.SetValue(value); }
    public static bool DefaultBuildMode { get => DefaultBuildModeOption.Value; set => DefaultBuildModeOption.SetValue(value); }
    public static bool AlwaysShowCrosshair { get => AlwaysShowCrosshairOption.Value; set => AlwaysShowCrosshairOption.SetValue(value); }
    public static bool ActionModeCrosshair { get => ActionModeCrosshairOption.Value; set => ActionModeCrosshairOption.SetValue(value); }
    public static float FieldOfView { get => FieldOfViewOption.Value; set => FieldOfViewOption.SetValue(value); }

    public static int AimOffsetX { get => (int)(Screen.width * (AimOffsetXOption.Value / 100)); set => AimOffsetXOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static int AimOffsetY { get => (int)(Screen.height * (AimOffsetYOption.Value / 100)); set => AimOffsetYOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static CameraAimMode CameraAimMode { get => CameraAimModeOption.GetEnumValue<CameraAimMode>(); set => CameraAimModeOption.SetValue((int)value); }

    public static bool LockZoom { get => LockCameraZoomOption.Value; set => LockCameraZoomOption.SetValue(value); }
    public static float LockZoomDistance { get => LockCameraZoomDistanceOption.Value; set => LockCameraZoomDistanceOption.SetValue(value); }
    public static float MinZoom { get => MinZoomOption.Value; set => MinZoomOption.SetValue(value); }
    public static float MaxZoom { get => MaxZoomOption.Value; set => MaxZoomOption.SetValue(value); }

    public static bool LockPitch { get => LockCamerMenuOptionstchOption.Value; set => LockCamerMenuOptionstchOption.SetValue(value); }
    public static float LockPitchAngle { get => LockCamerMenuOptionstchAngleOption.Value * Mathf.Deg2Rad; set => LockCamerMenuOptionstchAngleOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MinPitch { get => MinPitchOption.Value * Mathf.Deg2Rad; set => MinPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MaxPitch { get => MaxPitchOption.Value * Mathf.Deg2Rad; set => MaxPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }

    public static bool OverTheShoulder { get => OverTheShoulderOption.Value; set => OverTheShoulderOption.SetValue(value); }
    public static float OverTheShoulderX { get => OverTheShoulderXOption.Value; set => OverTheShoulderXOption.SetValue(value); }
    public static float OverTheShoulderY { get => OverTheShoulderYOption.Value; set => OverTheShoulderYOption.SetValue(value); }

    public static float FirstPersonForwardOffset = 1.65f;
    public static float MountedOffset = 1.6f;
    public static float HeadHeightOffset = 1.05f;
    public static float ShoulderRightOffset = 0.8f;

    const float ZoomOffset = 2f;

    public static readonly Dictionary<string, Vector2> FirstPersonShapeshiftOffsets = new()
    {
        { "AB_Shapeshift_Bat_Buff", new Vector2(0, 2.5f) },
        { "AB_Shapeshift_Bear_Buff", new Vector2(0.25f, 5f) },
        { "AB_Shapeshift_Bear_Skin01_Buff", new Vector2(0.25f, 5f) },
        { "AB_Shapeshift_Human_Grandma_Skin01_Buff", new Vector2(-0.1f, 1.55f) },
        { "AB_Shapeshift_Human_Buff", new Vector2(0.5f, 1.4f) },
        { "AB_Shapeshift_Rat_Buff", new Vector2(-1.85f, 2f) },
        { "AB_Shapeshift_Toad_Buff", new Vector2(-0.6f, 4.2f) },
        { "AB_Shapeshift_Wolf_Buff", new Vector2(-0.25f, 4.3f) },
        { "AB_Shapeshift_Wolf_Skin01_Buff", new Vector2(-0.25f, 4.3f) }
    };

    static ToggleOption EnabledOption;
    static SliderOption FieldOfViewOption;
    static ToggleOption AlwaysShowCrosshairOption;
    static ToggleOption ActionModeCrosshairOption;
    static ToggleOption FirstPersonEnabledOption;
    static ToggleOption DefaultBuildModeOption;

    static DropdownOption CameraAimModeOption;
    static SliderOption AimOffsetXOption;
    static SliderOption AimOffsetYOption;

    static ToggleOption LockCameraZoomOption;
    static SliderOption LockCameraZoomDistanceOption;
    static SliderOption MinZoomOption;
    static SliderOption MaxZoomOption;

    static ToggleOption LockCamerMenuOptionstchOption;
    static SliderOption LockCamerMenuOptionstchAngleOption;
    static SliderOption MinPitchOption;
    static SliderOption MaxPitchOption;

    static ToggleOption OverTheShoulderOption;
    static SliderOption OverTheShoulderXOption;
    static SliderOption OverTheShoulderYOption;

    static KeybindMapping EnabledKeybind;
    static KeybindMapping ActionModeKeybind;
    static KeybindMapping HideUIKeybind;
    public static void Initialize()
    {
        SetupOptions();
        SetupKeybinds();
    }
    public static void AddEnabledListener(OnChange<bool> action) => EnabledOption.AddListener(action);
    public static void AddFieldOfViewListener(OnChange<float> action) => FieldOfViewOption.AddListener(action);
    public static void AddHideUIListener(KeyEvent action) => HideUIKeybind.AddKeyDownListener(action);
    static void SetupOptions()
    {
        OptionCategory optionCategory = OptionsManager.AddCategory("RetroCamera");

        EnabledOption = optionCategory.AddToggle("retrocamera.enabled", "Enabled", true);
        FirstPersonEnabledOption = optionCategory.AddToggle("retrocamera.firstperson", "FirstPerson", true);
        DefaultBuildModeOption = optionCategory.AddToggle("retrocamera.defaultbuildmode", "DefaultBuildModeCamera", true);
        AlwaysShowCrosshairOption = optionCategory.AddToggle("retrocamera.alwaysshowcrosshair", "AlwaysShowCrosshair", false);
        ActionModeCrosshairOption = optionCategory.AddToggle("retrocamera.actionmodecrosshair", "ActionModeCrosshair", false);
        FieldOfViewOption = optionCategory.AddSlider("retrocamera.fieldofview", "Field of View", 50, 90, 60);

        optionCategory.AddDivider("ThirdPersonAiming"); // "ThirdPersonAiming" Divider: Index 0 (First in the list)
        CameraAimModeOption = optionCategory.AddDropdown("retrocamera.aimmode", "Aim Mode", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));
        AimOffsetXOption = optionCategory.AddSlider("retrocamera.aimoffsetx", "Screen X% Offset ", -25, 25, 0);
        AimOffsetYOption = optionCategory.AddSlider("retrocamera.aimoffsety", "Screen Y% Offset", -25, 25, 0);

        optionCategory.AddDivider("ThirdPersonZoom"); // "ThirdPersonZoom" Divider: Index 3 (After 3 entries)
        MinZoomOption = optionCategory.AddSlider("retrocamera.minzoom", "Min Zoom", 1, 18, 2);
        MaxZoomOption = optionCategory.AddSlider("retrocamera.maxzoom", "Max Zoom", 3, 20, 18);
        LockCameraZoomOption = optionCategory.AddToggle("retrocamera.lockzoom", "Lock Camera Zoom", false);
        LockCameraZoomDistanceOption = optionCategory.AddSlider("retrocamera.lockzoomdistance", "Locked Camera Zoom Distance", 6, 20, 15);
         
        optionCategory.AddDivider("ThirdPersonPitch"); // "ThirdPersonPitch" Divider: Index 7 (After 4 more entries)
        MinPitchOption = optionCategory.AddSlider("retrocamera.minpitch", "MinPitch", 0, 90, 9);
        MaxPitchOption = optionCategory.AddSlider("retrocamera.maxpitch", "MaxPitch", 0, 90, 90);
        LockCamerMenuOptionstchOption = optionCategory.AddToggle("retrocamera.lockpitch", "Lock Camera Pitch", false);
        LockCamerMenuOptionstchAngleOption = optionCategory.AddSlider("retrocamera.lockpitchangle", "Locked Camera Pitch Angle", 0, 90, 60);

        optionCategory.AddDivider("OverShoulder"); // "OverShoulder" Divider: Index 11 (After 4 more entries)
        OverTheShoulderOption = optionCategory.AddToggle("retrocamera.overtheshoulder", "Use Over the Shoulder Offset", false);
        OverTheShoulderXOption = optionCategory.AddSlider("retrocamera.overtheshoulderx", "X Offset", 0.5f, 4, 1);
        OverTheShoulderYOption = optionCategory.AddSlider("retrocamera.overtheshouldery", "Y Offset", 1, 8, 1);

        MinZoomOption.AddListener(value =>
        {
            if (value + ZoomOffset > MaxZoom && value + ZoomOffset < MaxZoomOption.MaxValue)
                MaxZoomOption.SetValue(value + ZoomOffset);
            else if (value + ZoomOffset > MaxZoomOption.MaxValue)
                MinZoomOption.SetValue(MaxZoomOption.MaxValue - ZoomOffset);
        });

        MaxZoomOption.AddListener(value =>
        {
            if (value - ZoomOffset < MinZoom && value - ZoomOffset > MinZoomOption.MinValue)
                MinZoomOption.SetValue(value - ZoomOffset);
            else if (value - ZoomOffset < MinZoomOption.MinValue)
                MaxZoomOption.SetValue(MinZoomOption.MinValue + ZoomOffset);
        });

        MinPitchOption.AddListener(value =>
        {
            if (value > MaxPitchOption.Value && value < MaxPitchOption.MaxValue)
                MaxPitchOption.SetValue(value);
            else if (value > MaxPitchOption.MaxValue)
                MinPitchOption.SetValue(MaxPitchOption.MaxValue);
        });

        MaxPitchOption.AddListener(value =>
        {
            if (value < MinPitchOption.Value && value > MinPitchOption.MinValue)
                MinPitchOption.SetValue(value);
            else if (value < MinPitchOption.MinValue)
                MaxPitchOption.SetValue(MinPitchOption.MinValue);
        });

        Persistence.SaveOptions();
    }
    static void SetupKeybinds()
    {
        KeybindCategory keybindCategory = KeybindsManager.AddCategory("RetroCamera");

        EnabledKeybind = keybindCategory.AddKeyBinding("retrocamera.enabled", "RetroCamera", "ToggleRetroCamera", KeyCode.Comma);
        EnabledKeybind.AddKeyDownListener(() =>
        {
            Core.Log.LogInfo(keybindCategory.Name + " Enabled: " + !Enabled);

            EnabledOption.SetValue(!Enabled);
        });

        ActionModeKeybind = keybindCategory.AddKeyBinding("retrocamera.actionmode", "RetroCamera", "ToggleActionMode", KeyCode.Period);
        ActionModeKeybind.AddKeyDownListener(() =>
        {
            if (Enabled && !IsFirstPerson)
            {
                Core.Log.LogInfo($"Start: Action Mode: {IsActionMode}; Mouse Locked: {IsMouseLocked}; Wheel Visible: {ActionWheelSystemPatch.WheelVisible}; IsMenuOpen: {IsMenuOpen}");
                
                IsMouseLocked = !IsMouseLocked;
                IsActionMode = !IsActionMode;

                if (IsMenuOpen) IsMenuOpen = false;

                if (ActionWheelSystemPatch.WheelVisible) ActionWheelSystemPatch.WheelVisible = false;

                Core.Log.LogInfo($"End: Action Mode: {IsActionMode}; Mouse Locked: {IsMouseLocked}; Wheel Visible: {ActionWheelSystemPatch.WheelVisible}; IsMenuOpen: {IsMenuOpen}");
            }
        });

        HideUIKeybind = keybindCategory.AddKeyBinding("retrocamera.hideui", "RetroCamera", "Hide UI", KeyCode.Slash);

        Persistence.SaveKeybinds();
    }
}
