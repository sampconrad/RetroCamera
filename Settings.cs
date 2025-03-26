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
    public static bool Enabled { get => _enabledOption.Value; set => _enabledOption.SetValue(value); }
    public static bool FirstPersonEnabled { get => _firstPersonEnabledOption.Value; set => _firstPersonEnabledOption.SetValue(value); }
    public static bool DefaultBuildMode { get => _defaultBuildModeOption.Value; set => _defaultBuildModeOption.SetValue(value); }
    public static bool AlwaysShowCrosshair { get => _alwaysShowCrosshairOption.Value; set => _alwaysShowCrosshairOption.SetValue(value); }
    public static bool ActionModeCrosshair { get => _actionModeCrosshairOption.Value; set => _actionModeCrosshairOption.SetValue(value); }
    public static float FieldOfView { get => _fieldOfViewOption.Value; set => _fieldOfViewOption.SetValue(value); }

    public static int AimOffsetX { get => (int)(Screen.width * (_aimOffsetXOption.Value / 100)); set => _aimOffsetXOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static int AimOffsetY { get => (int)(Screen.height * (_aimOffsetYOption.Value / 100)); set => _aimOffsetYOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static CameraAimMode CameraAimMode { get => _cameraAimModeOption.GetEnumValue<CameraAimMode>(); set => _cameraAimModeOption.SetValue((int)value); }

    public static bool LockZoom { get => _lockCameraZoomOption.Value; set => _lockCameraZoomOption.SetValue(value); }
    public static float LockZoomDistance { get => _lockCameraZoomDistanceOption.Value; set => _lockCameraZoomDistanceOption.SetValue(value); }
    public static float MinZoom { get => _minZoomOption.Value; set => _minZoomOption.SetValue(value); }
    public static float MaxZoom { get => _maxZoomOption.Value; set => _maxZoomOption.SetValue(value); }

    public static bool LockPitch { get => _lockCamerMenuOptionstchOption.Value; set => _lockCamerMenuOptionstchOption.SetValue(value); }
    public static float LockPitchAngle { get => _lockCamerMenuOptionstchAngleOption.Value * Mathf.Deg2Rad; set => _lockCamerMenuOptionstchAngleOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MinPitch { get => _minPitchOption.Value * Mathf.Deg2Rad; set => _minPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MaxPitch { get => _maxPitchOption.Value * Mathf.Deg2Rad; set => _maxPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }

    public static bool OverTheShoulder { get => _overTheShoulderOption.Value; set => _overTheShoulderOption.SetValue(value); }
    public static float OverTheShoulderX { get => _overTheShoulderXOption.Value; set => _overTheShoulderXOption.SetValue(value); }
    public static float OverTheShoulderY { get => _overTheShoulderYOption.Value; set => _overTheShoulderYOption.SetValue(value); }

    public static float _firstPersonForwardOffset = 1.65f;
    public static float _mountedOffset = 1.6f;
    public static float _headHeightOffset = 1.05f;
    public static float _shoulderRightOffset = 0.8f;

    const float ZOOM_OFFSET = 2f;

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

    static ToggleOption _enabledOption;
    static SliderOption _fieldOfViewOption;
    static ToggleOption _alwaysShowCrosshairOption;
    static ToggleOption _actionModeCrosshairOption;
    static ToggleOption _firstPersonEnabledOption;
    static ToggleOption _defaultBuildModeOption;

    static DropdownOption _cameraAimModeOption;
    static SliderOption _aimOffsetXOption;
    static SliderOption _aimOffsetYOption;

    static ToggleOption _lockCameraZoomOption;
    static SliderOption _lockCameraZoomDistanceOption;
    static SliderOption _minZoomOption;
    static SliderOption _maxZoomOption;

    static ToggleOption _lockCamerMenuOptionstchOption;
    static SliderOption _lockCamerMenuOptionstchAngleOption;
    static SliderOption _minPitchOption;
    static SliderOption _maxPitchOption;

    static ToggleOption _overTheShoulderOption;
    static SliderOption _overTheShoulderXOption;
    static SliderOption _overTheShoulderYOption;

    static Keybind _enabledKeybind;
    static Keybind _actionModeKeybind;
    static Keybind _hideUIKeybind;
    static Keybind _holdToScrollKeybind;
    public static void Initialize()
    {
        SetupOptions();
        SetupKeybinds();
    }
    public static void AddEnabledListener(OnChangeHandler<bool> action) => _enabledOption.AddListener(action);
    public static void AddFieldOfViewListener(OnChangeHandler<float> action) => _fieldOfViewOption.AddListener(action);
    public static void AddHideUIListener(KeyEventHandler action) => _hideUIKeybind.AddKeyDownListener(action);
    static void SetupOptions()
    {
        OptionCategory optionCategory = OptionsManager.AddCategory("RetroCamera");

        _enabledOption = optionCategory.AddToggle("retrocamera.enabled", "Enabled", true);
        _firstPersonEnabledOption = optionCategory.AddToggle("retrocamera.firstperson", "FirstPerson", true);
        _defaultBuildModeOption = optionCategory.AddToggle("retrocamera.defaultbuildmode", "DefaultBuildModeCamera", true);
        _alwaysShowCrosshairOption = optionCategory.AddToggle("retrocamera.alwaysshowcrosshair", "AlwaysShowCrosshair", false);
        _actionModeCrosshairOption = optionCategory.AddToggle("retrocamera.actionmodecrosshair", "ActionModeCrosshair", false);
        _fieldOfViewOption = optionCategory.AddSlider("retrocamera.fieldofview", "Field of View", 50, 90, 60);

        optionCategory.AddDivider("ThirdPersonAiming"); // "ThirdPersonAiming" Divider: Index 0 (First in the list)
        _cameraAimModeOption = optionCategory.AddDropdown("retrocamera.aimmode", "Aim Mode", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));
        _aimOffsetXOption = optionCategory.AddSlider("retrocamera.aimoffsetx", "Screen X% Offset ", -25, 25, 0);
        _aimOffsetYOption = optionCategory.AddSlider("retrocamera.aimoffsety", "Screen Y% Offset", -25, 25, 0);

        optionCategory.AddDivider("ThirdPersonZoom"); // "ThirdPersonZoom" Divider: Index 3 (After 3 entries)
        _minZoomOption = optionCategory.AddSlider("retrocamera.minzoom", "Min Zoom", 1, 18, 2);
        _maxZoomOption = optionCategory.AddSlider("retrocamera.maxzoom", "Max Zoom", 3, 20, 18);
        _lockCameraZoomOption = optionCategory.AddToggle("retrocamera.lockzoom", "Lock Camera Zoom", false);
        _lockCameraZoomDistanceOption = optionCategory.AddSlider("retrocamera.lockzoomdistance", "Locked Camera Zoom Distance", 6, 20, 15);

        optionCategory.AddDivider("ThirdPersonPitch"); // "ThirdPersonPitch" Divider: Index 7 (After 4 more entries)
        _minPitchOption = optionCategory.AddSlider("retrocamera.minpitch", "MinPitch", 0, 90, 9);
        _maxPitchOption = optionCategory.AddSlider("retrocamera.maxpitch", "MaxPitch", 0, 90, 90);
        _lockCamerMenuOptionstchOption = optionCategory.AddToggle("retrocamera.lockpitch", "Lock Camera Pitch", false);
        _lockCamerMenuOptionstchAngleOption = optionCategory.AddSlider("retrocamera.lockpitchangle", "Locked Camera Pitch Angle", 0, 90, 60);

        optionCategory.AddDivider("OverShoulder"); // "OverShoulder" Divider: Index 11 (After 4 more entries)
        _overTheShoulderOption = optionCategory.AddToggle("retrocamera.overtheshoulder", "Use Over the Shoulder Offset", false);
        _overTheShoulderXOption = optionCategory.AddSlider("retrocamera.overtheshoulderx", "X Offset", 0.5f, 4, 1);
        _overTheShoulderYOption = optionCategory.AddSlider("retrocamera.overtheshouldery", "Y Offset", 1, 8, 1);

        _minZoomOption.AddListener(value =>
        {
            if (value + ZOOM_OFFSET > MaxZoom && value + ZOOM_OFFSET < _maxZoomOption.MaxValue)
                _maxZoomOption.SetValue(value + ZOOM_OFFSET);
            else if (value + ZOOM_OFFSET > _maxZoomOption.MaxValue)
                _minZoomOption.SetValue(_maxZoomOption.MaxValue - ZOOM_OFFSET);
        });

        _maxZoomOption.AddListener(value =>
        {
            if (value - ZOOM_OFFSET < MinZoom && value - ZOOM_OFFSET > _minZoomOption.MinValue)
                _minZoomOption.SetValue(value - ZOOM_OFFSET);
            else if (value - ZOOM_OFFSET < _minZoomOption.MinValue)
                _maxZoomOption.SetValue(_minZoomOption.MinValue + ZOOM_OFFSET);
        });

        _minPitchOption.AddListener(value =>
        {
            if (value > _maxPitchOption.Value && value < _maxPitchOption.MaxValue)
                _maxPitchOption.SetValue(value);
            else if (value > _maxPitchOption.MaxValue)
                _minPitchOption.SetValue(_maxPitchOption.MaxValue);
        });

        _maxPitchOption.AddListener(value =>
        {
            if (value < _minPitchOption.Value && value > _minPitchOption.MinValue)
                _minPitchOption.SetValue(value);
            else if (value < _minPitchOption.MinValue)
                _maxPitchOption.SetValue(_minPitchOption.MinValue);
        });

        Persistence.SaveOptions();
    }
    static void SetupKeybinds()
    {
        KeybindCategory keybindCategory = KeybindsManager.AddCategory("RetroCamera");

        _enabledKeybind = AddKeyBinding("retrocamera.enabled", "RetroCamera", "ToggleRetroCamera", KeyCode.LeftBracket);
        _enabledKeybind.AddKeyDownListener(() =>
        {
            Core.Log.LogInfo(keybindCategory.Name + " Enabled: " + !Enabled);

            _enabledOption.SetValue(!Enabled);
        });

        _actionModeKeybind = AddKeyBinding("retrocamera.actionmode", "RetroCamera", "ToggleActionMode", KeyCode.RightBracket);
        _actionModeKeybind.AddKeyDownListener(() =>
        {
            if (Enabled && !_isFirstPerson)
            {
                Core.Log.LogInfo($"Start: Action Mode: {_isActionMode}; Mouse Locked: {_isMouseLocked}; Wheel Visible: {ActionWheelSystemPatch._wheelVisible}; IsMenuOpen: {IsMenuOpen}");

                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;

                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;

                Core.Log.LogInfo($"End: Action Mode: {_isActionMode}; Mouse Locked: {_isMouseLocked}; Wheel Visible: {ActionWheelSystemPatch._wheelVisible}; IsMenuOpen: {IsMenuOpen}");
            }
        });

        _hideUIKeybind = AddKeyBinding("retrocamera.hideui", "RetroCamera", "HideUI", KeyCode.Backslash);

        /*
        _holdToScrollKeybind = keybindCategory.AddKeyBinding("retrocamera.holdtoscroll", "RetroCamera", "HoldToScroll", KeyCode.RightAlt);
        _holdToScrollKeybind.AddKeyPressedListener(() =>
        {
            _chatScroll = true; // stop zooming in and out while held, act as if mouse is hovering over scrollbar in chat window
            LockZoom = true;
            _hudChatWindow.ScrollView.m_VerticalScrollbar.isPointerInside = true;
        });

        _holdToScrollKeybind.AddKeyUpListener(() =>
        {
            _chatScroll = false;
            LockZoom = false;
            _hudChatWindow.ScrollView.m_VerticalScrollbar.isPointerInside = false;
        });
        */

        Persistence.SaveKeybinds();
    }
}
