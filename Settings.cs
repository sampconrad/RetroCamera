using RetroCamera.Configuration;
using RetroCamera.Patches;
using UnityEngine;
using static RetroCamera.Configuration.Keybinding;
using static RetroCamera.Configuration.Keybinding.Keybinding;
using static RetroCamera.Utilities.CameraState;
using static RetroCamera.Utilities.Persistence;

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

    static Toggle _enabledOption;
    static Slider _fieldOfViewOption;
    static Toggle _alwaysShowCrosshairOption;
    static Toggle _actionModeCrosshairOption;
    static Toggle _firstPersonEnabledOption;
    static Toggle _defaultBuildModeOption;

    static Dropdown _cameraAimModeOption;
    static Slider _aimOffsetXOption;
    static Slider _aimOffsetYOption;

    static Toggle _lockCameraZoomOption;
    static Slider _lockCameraZoomDistanceOption;
    static Slider _minZoomOption;
    static Slider _maxZoomOption;

    static Toggle _lockCamerMenuOptionstchOption;
    static Slider _lockCamerMenuOptionstchAngleOption;
    static Slider _minPitchOption;
    static Slider _maxPitchOption;

    static Toggle _overTheShoulderOption;
    static Slider _overTheShoulderXOption;
    static Slider _overTheShoulderYOption;

    static Keybind _enabledKeybind;
    static Keybind _actionModeKeybind;
    static Keybind _hideHUDKeybind;
    // static Keybind _holdToScrollKeybind;

    const string ENABLE_KEY = "Enable";
    const string ACTION_KEY = "Action Mode";
    const string HUD_KEY = "Toggle HUD";
    public static void Initialize()
    {
        try
        {
            if (!SetupSavedOptions()) SetupDefaultOptions();
            if (!SetupSavedKeybinds()) SetupDefaultKeybinds();
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
    public static void AddEnabledListener(SettingChangedHandler<bool> action) => _enabledOption.AddListener(action);
    public static void AddFieldOfViewListener(SettingChangedHandler<float> action) => _fieldOfViewOption.AddListener(action);
    public static void AddHideHUDListener(KeybindHandler action) => _hideHUDKeybind.AddKeyDownListener(action);
    static void SetupDefaultOptions()
    {
        OptionSettings optionCategory = OptionManager.AddCategory("Retro Camera");

        // Basic enable/disable toggle for the RetroCamera system
        _enabledOption = optionCategory.AddToggle("Enabled", "Enable or disable RetroCamera", true);

        // Enables zooming in far enough to support first-person perspective
        _firstPersonEnabledOption = optionCategory.AddToggle("First Person", "Toggle being able to zoom in far enough for first person view/perspective", true);

        // Overrides the default camera behavior when entering build mode
        _defaultBuildModeOption = optionCategory.AddToggle("Build Mode", "Toggle using RetroCamera behaviour during build mode", true);

        // Keeps the crosshair visible at all times, regardless of camera mode or zoom
        _alwaysShowCrosshairOption = optionCategory.AddToggle("Always Show Crosshair", "Keeps crosshair visible even when zoomed out or idle", false);

        // Displays the crosshair only when performing combat or interaction actions
        _actionModeCrosshairOption = optionCategory.AddToggle("Action Mode Crosshair", "Shows the crosshair during action mode if not already always showing", false);

        // Adjusts the field of view (FOV) for the camera
        _fieldOfViewOption = optionCategory.AddSlider("FOV", "Adjust camera field of view", 50, 90, 60);

        optionCategory.AddDivider("Third Person Aiming");

        // Sets the aiming behavior for third person (e.g., classic, over-the-shoulder)
        _cameraAimModeOption = optionCategory.AddDropdown("Aiming Mode", "Select the aiming style used for abilities", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));

        // Shifts the camera's aim left or right relative to center
        _aimOffsetXOption = optionCategory.AddSlider("Horizontal Offset", "Adjust the horizontal aiming position offset", -25, 25, 0);

        // Raises or lowers the aim point relative to center
        _aimOffsetYOption = optionCategory.AddSlider("Vertical Offset", "Adjust the vertical aiming position offset", -25, 25, 0);

        optionCategory.AddDivider("Third Person Zoom");

        // Minimum allowed zoom distance from the character
        _minZoomOption = optionCategory.AddSlider("Min Zoom", "Sets how close the camera can zoom in to the player", 1, 18, 2);

        // Maximum allowed zoom distance from the character
        _maxZoomOption = optionCategory.AddSlider("Max Zoom", "Sets how far the camera can zoom out from the player", 3, 20, 18);

        // Prevents the player from changing the zoom level dynamically
        _lockCameraZoomOption = optionCategory.AddToggle("Lock Zoom", "Locks camera zoom at a fixed distance", false);

        // Defines the exact zoom distance used when zoom is locked
        _lockCameraZoomDistanceOption = optionCategory.AddSlider("Locked Zoom Distance", "Zoom distance to maintain when locked", 6, 20, 15);

        optionCategory.AddDivider("Third Person Pitch");

        // Lowest pitch angle the camera can rotate down to
        _minPitchOption = optionCategory.AddSlider("Min Pitch", "Restricts how far the camera can tilt downward", 0, 90, 9);

        // Highest pitch angle the camera can rotate upward
        _maxPitchOption = optionCategory.AddSlider("Max Pitch", "Restricts how far the camera can tilt upward", 0, 90, 90);

        // Disables camera pitch movement, forcing it to remain fixed
        _lockCamerMenuOptionstchOption = optionCategory.AddToggle("Lock Pitch", "Locks camera pitch at a fixed angle", false);

        // Sets the pitch angle used when camera pitch is locked
        _lockCamerMenuOptionstchAngleOption = optionCategory.AddSlider("Locked Pitch Angle", "Pitch angle to maintain when locked", 0, 90, 60);

        optionCategory.AddDivider("Over Shoulder");

        // Activates camera offset for over-the-shoulder perspective
        _overTheShoulderOption = optionCategory.AddToggle("Enable Shoulder Offset", "Enables third-person over-the-shoulder view offset", false);

        // Adjusts the horizontal distance from the character in over-the-shoulder mode
        _overTheShoulderXOption = optionCategory.AddSlider("Horizontal Offset", "Controls sideways camera position offset for shoulder view", 0.5f, 4, 1);

        // Adjusts the vertical camera height in over-the-shoulder mode
        _overTheShoulderYOption = optionCategory.AddSlider("Vertical Offset", "Controls upward/downward camera position offset for shoulder view", 1, 8, 1);

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

        SaveOptions();
    }
    static void SetupDefaultKeybinds()
    {
        Configuration.Keybinding.Keybinding keybindCategory = KeybindManager.AddCategory("Retro Camera");

        _enabledKeybind = AddKeyBinding(ENABLE_KEY, "Retro Camera", "Enable RetroCamera functions", KeyCode.LeftBracket);
        _enabledKeybind.AddKeyDownListener(() =>
        {
            // Core.Log.LogInfo(keybindCategory.Name + " Enabled: " + !Enabled);

            _enabledOption.SetValue(!Enabled);
        });

        _actionModeKeybind = AddKeyBinding(ACTION_KEY, "Retro Camera", "Toggle action mode", KeyCode.RightBracket);
        _actionModeKeybind.AddKeyDownListener(() =>
        {
            if (Enabled && !_isFirstPerson)
            {
                // Core.Log.LogInfo($"Start: Action Mode: {_isActionMode}; Mouse Locked: {_isMouseLocked}; Wheel Visible: {ActionWheelSystemPatch._wheelVisible}; IsMenuOpen: {IsMenuOpen}");

                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;

                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;

                // Core.Log.LogInfo($"End: Action Mode: {_isActionMode}; Mouse Locked: {_isMouseLocked}; Wheel Visible: {ActionWheelSystemPatch._wheelVisible}; IsMenuOpen: {IsMenuOpen}");
            }
        });

        _hideHUDKeybind = AddKeyBinding(HUD_KEY, "Retro Camera", "Toggle HUD visibility", KeyCode.Backslash);

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

        SaveKeybinds();
    }
    public static bool SetupSavedKeybinds()
    {
        var loaded = LoadKeybinds();
        if (loaded != null)
        {

            return true;
        }

        return false;
    }
    public static bool SetupSavedOptions()
    {
        var loaded = LoadOptions();
        if (loaded != null)
        {
            OptionSettings optionCategory = OptionManager.AddCategory("Retro Camera");

            _enabledOption = optionCategory.AddToggle("Enabled", "Enable or disable RetroCamera", true);
            _firstPersonEnabledOption = optionCategory.AddToggle("First Person", "Toggle being able to zoom in far enough for first person view/perspective", true);
            _defaultBuildModeOption = optionCategory.AddToggle("Build Mode", "Toggle using RetroCamera behaviour during build mode", true);
            _alwaysShowCrosshairOption = optionCategory.AddToggle("Always Show Crosshair", "Keeps crosshair visible even when zoomed out or idle", false);
            _actionModeCrosshairOption = optionCategory.AddToggle("Action Mode Crosshair", "Shows the crosshair during action mode if not already always showing", false);
            _fieldOfViewOption = optionCategory.AddSlider("FOV", "Adjust camera field of view", 50, 90, 60);
            optionCategory.AddDivider("Third Person Aiming");

            _cameraAimModeOption = optionCategory.AddDropdown("Aiming Mode", "Select the aiming style used for abilities", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));
            _aimOffsetXOption = optionCategory.AddSlider("Horizontal Offset", "Adjust the horizontal aiming position offset", -25, 25, 0);
            _aimOffsetYOption = optionCategory.AddSlider("Vertical Offset", "Adjust the vertical aiming position offset", -25, 25, 0);
            optionCategory.AddDivider("Third Person Zoom");

            _minZoomOption = optionCategory.AddSlider("Min Zoom", "Sets how close the camera can zoom in to the player", 1, 18, 2);
            _maxZoomOption = optionCategory.AddSlider("Max Zoom", "Sets how far the camera can zoom out from the player", 3, 20, 18);
            _lockCameraZoomOption = optionCategory.AddToggle("Lock Zoom", "Locks camera zoom at a fixed distance", false);
            _lockCameraZoomDistanceOption = optionCategory.AddSlider("Locked Zoom Distance", "Zoom distance to maintain when locked", 6, 20, 15);
            optionCategory.AddDivider("Third Person Pitch");

            _minPitchOption = optionCategory.AddSlider("Min Pitch", "Restricts how far the camera can tilt downward", 0, 90, 9);
            _maxPitchOption = optionCategory.AddSlider("Max Pitch", "Restricts how far the camera can tilt upward", 0, 90, 90);
            _lockCamerMenuOptionstchOption = optionCategory.AddToggle("Lock Pitch", "Locks camera pitch at a fixed angle", false);
            _lockCamerMenuOptionstchAngleOption = optionCategory.AddSlider("Locked Pitch Angle", "Pitch angle to maintain when locked", 0, 90, 60);
            optionCategory.AddDivider("Over Shoulder");

            _overTheShoulderOption = optionCategory.AddToggle("Enable Shoulder Offset", "Enables third-person over-the-shoulder view offset", false);
            _overTheShoulderXOption = optionCategory.AddSlider("Horizontal Offset", "Controls sideways camera position offset for shoulder view", 0.5f, 4, 1);
            _overTheShoulderYOption = optionCategory.AddSlider("Vertical Offset", "Controls upward/downward camera position offset for shoulder view", 1, 8, 1);

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

            return true;
        }

        return false;
    }
}
