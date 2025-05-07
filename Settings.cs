﻿using RetroCamera.Configuration;
using RetroCamera.Patches;
using UnityEngine;
using static RetroCamera.Utilities.CameraState;
using static RetroCamera.Utilities.Persistence;
using static RetroCamera.Configuration.OptionsManager;
using BoolChanged = RetroCamera.Configuration.MenuOption<bool>.OptionChangedHandler<bool>;
using FloatChanged = RetroCamera.Configuration.MenuOption<float>.OptionChangedHandler<float>;
using static RetroCamera.Configuration.KeybindsManager;
using static RetroCamera.Configuration.Keybinding;

namespace RetroCamera;
internal static class Settings
{
    public static bool Enabled { get => _enabledOption.Value; set => _enabledOption.SetValue(value); }
    public static bool FirstPersonEnabled { get => _firstPersonEnabledOption.Value; set => _firstPersonEnabledOption.SetValue(value); }
    public static bool AlwaysShowCrosshair { get => _alwaysShowCrosshairOption.Value; set => _alwaysShowCrosshairOption.SetValue(value); }
    public static bool ActionModeCrosshair { get => _actionModeCrosshairOption.Value; set => _actionModeCrosshairOption.SetValue(value); }
    public static bool ActionModeControlsRetroCamera { get => _actionModeControlsRetroCameraOption.Value; set => _actionModeControlsRetroCameraOption.SetValue(value); }
    public static bool UseDotCrosshair { get => _useDotCrosshairOption.Value; set => _useDotCrosshairOption.SetValue(value); }
    public static float FieldOfView { get => _fieldOfViewOption.Value; set => _fieldOfViewOption.SetValue(value); }
    public static int AimOffsetX { get => (int)(Screen.width * (_aimOffsetXOption.Value / 100)); set => _aimOffsetXOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static int AimOffsetY { get => (int)(Screen.height * (_aimOffsetYOption.Value / 100)); set => _aimOffsetYOption.SetValue(Mathf.Clamp(value / Screen.width, -25, 25)); }
    public static bool LockZoom { get => _lockCameraZoomOption.Value; set => _lockCameraZoomOption.SetValue(value); }
    public static float LockZoomDistance { get => _lockCameraZoomDistanceOption.Value; set => _lockCameraZoomDistanceOption.SetValue(value); }
    public static float MinZoom { get => _minZoomOption.Value; set => _minZoomOption.SetValue(value); }
    public static float MaxZoom { get => _maxZoomOption.Value; set => _maxZoomOption.SetValue(value); }
    public static bool LockPitch { get => _lockCamerPitchOption.Value; set => _lockCamerPitchOption.SetValue(value); }
    public static float LockPitchAngle { get => _lockCameraPitchAngleOption.Value * Mathf.Deg2Rad; set => _lockCameraPitchAngleOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MinPitch { get => _minPitchOption.Value * Mathf.Deg2Rad; set => _minPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static float MaxPitch { get => _maxPitchOption.Value * Mathf.Deg2Rad; set => _maxPitchOption.SetValue(Mathf.Clamp(value * Mathf.Rad2Deg, 0, 90)); }
    public static bool OverTheShoulder { get => _overTheShoulderOption.Value; set => _overTheShoulderOption.SetValue(value); }
    public static float OverTheShoulderX { get => _overTheShoulderXOption.Value; set => _overTheShoulderXOption.SetValue(value); }
    public static float OverTheShoulderY { get => _overTheShoulderYOption.Value; set => _overTheShoulderYOption.SetValue(value); }

    public const float FIRST_PERSON_FORWARD_OFFSET = 1.65f;
    public const float MOUNTED_OFFSET = 1.6f;
    public const float HEAD_HEIGHT_OFFSET = 1.05f;
    public const float SHOULDER_RIGHT_OFFSET = 0.8f;

    const float ZOOM_OFFSET = 2f;

    /* unused, if overall merited will do actual buff checks
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
    */

    static Toggle _enabledOption;
    static Slider _fieldOfViewOption;
    static Toggle _alwaysShowCrosshairOption;
    static Toggle _actionModeCrosshairOption;
    static Toggle _actionModeControlsRetroCameraOption;
    static Toggle _useDotCrosshairOption;
    static Toggle _firstPersonEnabledOption;
    // static Toggle _defaultBuildModeOption;

    // static Dropdown _cameraAimModeOption;
    static Slider _aimOffsetXOption;
    static Slider _aimOffsetYOption;

    static Toggle _lockCameraZoomOption;
    static Slider _lockCameraZoomDistanceOption;
    static Slider _minZoomOption;
    static Slider _maxZoomOption;

    static Toggle _lockCamerPitchOption;
    static Slider _lockCameraPitchAngleOption;
    static Slider _minPitchOption;
    static Slider _maxPitchOption;

    static Toggle _overTheShoulderOption;
    static Slider _overTheShoulderXOption;
    static Slider _overTheShoulderYOption;

    static Keybinding _enabledKeybind;
    static Keybinding _actionModeKeybind;
    static Keybinding _toggleHUDKeybind;
    static Keybinding _toggleFogKeybind;
    static Keybinding _completeTutorialKeybind;
    static Keybinding _cycleCameraKeybind; // WIP
    public static void Initialize()
    {
        try
        {
            RegisterOptions();
            RegisterKeybinds();

            TryLoadOptions();
            TryLoadKeybinds();

            SaveOptions();
            SaveKeybinds();
        }
        catch (Exception ex)
        {
            Core.Log.LogError(ex);
        }
    }
    public static void AddEnabledListener(BoolChanged handler) =>
    _enabledOption.AddListener(handler);
    public static void AddFieldOfViewListener(FloatChanged handler) =>
        _fieldOfViewOption.AddListener(handler);
    public static void AddHideHUDListener(KeyHandler action) => 
        _toggleHUDKeybind.AddKeyDownListener(action);
    public static void AddHideFogListener(KeyHandler action) =>
        _toggleFogKeybind.AddKeyDownListener(action);
    public static void AddCompleteTutorialListener(KeyHandler action) =>
        _completeTutorialKeybind.AddKeyDownListener(action);
    public static void AddCycleCameraListener(KeyHandler action) =>
        _cycleCameraKeybind.AddKeyDownListener(action);

    static void RegisterOptions()
    {
        // Core.Log.LogWarning("Registering options...");

        _enabledOption = AddToggle("Enabled", "Enable or disable RetroCamera", true);
        _firstPersonEnabledOption = AddToggle("First Person", "Enable zooming in far enough for first-person view", true);
        _alwaysShowCrosshairOption = AddToggle("Always Show Crosshair", "Keep crosshair visible always", false);
        _actionModeCrosshairOption = AddToggle("Action Mode Crosshair", "Show crosshair during action mode", false);
        _actionModeControlsRetroCameraOption = AddToggle("Action Mode Controls Retro Camera", "Action Mode keybind toggle will also control Retro Camera", false);
        _useDotCrosshairOption = AddToggle("Use Dot Crosshair", "Use a simple white dot instead of the default cursor texture", false);
        _useDotCrosshairOption.AddListener(_ => Systems.RetroCamera.RebuildCrosshair());
        _fieldOfViewOption = AddSlider("FOV", "Camera field of view", 50, 90, 60);

        AddDivider("Third Person Zoom");
        _minZoomOption = AddSlider("Min Zoom", "Minimum zoom", 1f, 15f, 1f);
        _maxZoomOption = AddSlider("Max Zoom", "Maximum zoom", 5f, 20f, 15f);
        _lockCameraZoomOption = AddToggle("Lock Zoom", "Lock zoom distance", false);
        _lockCameraZoomDistanceOption = AddSlider("Locked Zoom Distance", "Fixed zoom distance when locked", 1f, 20f, 15f);

        AddDivider("Third Person Pitch");
        _minPitchOption = AddSlider("Min Pitch", "Minimum camera pitch", 0f, 90f, 10f);
        _maxPitchOption = AddSlider("Max Pitch", "Maximum camera pitch", 0f, 90f, 90f);
        _lockCamerPitchOption = AddToggle("Lock Pitch", "Lock camera pitch", false);
        _lockCameraPitchAngleOption = AddSlider("Locked Pitch Angle", "Fixed pitch angle when locked", 0f, 90f, 60f);

        AddDivider("Third Person Aiming");
        // _cameraAimModeOption = AddDropdown("Aiming Mode", "Ability aiming style", (int)CameraAimMode.Default, Enum.GetNames(typeof(CameraAimMode)));
        _aimOffsetXOption = AddSlider("Aiming Horizontal Offset", "Aim horizontal offset", -25f, 25f, 0f);
        _aimOffsetYOption = AddSlider("Aiming Vertical Offset", "Aim vertical offset", -25f, 25f, 0f);

        AddDivider("Over Shoulder");
        _overTheShoulderOption = AddToggle("Enable Shoulder Offset", "Enable over-the-shoulder camera", false);
        _overTheShoulderXOption = AddSlider("Shoulder Horizontal Offset", "Shoulder view horizontal offset", 1f, 4f, 1f);
        _overTheShoulderYOption = AddSlider("Shoulder Vertical Offset", "Shoulder view vertical offset", 1f, 8f, 1f);

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
    }
    static void RegisterKeybinds()
    {
        // Core.Log.LogWarning("Registering keybinds...");

        _enabledKeybind = AddKeybind("Toggle RetroCamera", "Enable or disable RetroCamera functions", KeyCode.LeftBracket);
        _enabledKeybind.AddKeyDownListener(() =>
        {
            _enabledOption.SetValue(!Enabled);
        });

        _actionModeKeybind = AddKeybind("Toggle Action Mode", "Toggle action mode", KeyCode.RightBracket);
        _actionModeKeybind.AddKeyDownListener(() =>
        {
            if (!_isFirstPerson)
            {
                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                if (IsMenuOpen) IsMenuOpen = false;
                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;
                if (Cursor.lockState.Equals(CursorLockMode.Locked) && (!_isActionMode || !_isMouseLocked))
                {
                    Cursor.lockState = CursorLockMode.None;
                }

                // FORCES RETRO CAMERA ON/OFF WHEN TOGGLING ACTION MODE
                if (Settings.ActionModeControlsRetroCamera)
                {
                    if (_isActionMode && !Settings.Enabled)
                    {
                        Settings.Enabled = true;
                        Systems.RetroCamera.Enabled(true);
                    }
                    else if (!_isActionMode && Settings.Enabled)
                    {
                        Settings.Enabled = false;
                        Systems.RetroCamera.Enabled(false);
                    }
                }
            }
        });

        _toggleHUDKeybind = AddKeybind("Toggle HUD", "Toggle HUD visibility", KeyCode.Backslash);

        _toggleFogKeybind = AddKeybind("Toggle Fog/Clouds", "Toggle visibility of fog and clouds (cloud ground shadows also affected)", KeyCode.Equals);

        _completeTutorialKeybind = AddKeybind("Complete Tutorial", "Pushes button for completed tutorials if applicable", KeyCode.Minus);

        // _cycleCameraKeybind = AddKeybind("Cycle Camera", "Cycle active camera (topdown, orbit, hybrid, free)", KeyCode.Minus);
    }
    public static bool TryLoadOptions()
    {
        var loaded = LoadOptions();
        if (loaded == null)
            return false;

        foreach (var (key, loadedOption) in loaded)
        {
            if (Options.TryGetValue(key, out var registeredOption))
                registeredOption.ApplySaved(loadedOption);
        }

        return true;
    }
    public static bool TryLoadKeybinds()
    {
        var loaded = LoadKeybinds();
        if (loaded == null) return false;

        foreach (var (key, loadedBind) in loaded)
        {
            if (Keybinds.TryGetValue(key, out var registeredBind))
                registeredBind.ApplySaved(loadedBind);
        }

        return true;
    }
}