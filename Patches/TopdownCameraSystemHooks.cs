using BepInEx.Unity.IL2CPP.Hook;
using ProjectM;
using ProjectM.Presentation;
using RetroCamera.Behaviours;
using RetroCamera.Utilities;
using System.Runtime.InteropServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Patches;
#nullable enable
internal static class TopdownCameraSystemHooks
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void HandleInputHandler(IntPtr _this, ref InputState inputState);
    static HandleInputHandler? _handleInputOriginal;
    static INativeDetour? _handleInputDetour;

    /*
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void UpdateCameraInputsHandler(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData);
    static UpdateCameraInputsHandler? _updateCameraInputsOriginal;
    static INativeDetour? _updateCameraInputsDetour;
    */

    // CursorPositionSystem
    // CursorPositionSystem_59A0B5A3_LambdaJob_0_Execute
    // public unsafe void CursorPositionSystem_59A0B5A3_LambdaJob_0_Execute([DefaultParameterValue(null)] ref CollisionWorld collisionWorld, [DefaultParameterValue(null)] ref int heightLevel, [DefaultParameterValue(null)] ref FadeTargetsSingleton fadeTargets, [DefaultParameterValue(null)] ref CurrentFadingDataSingleton fadeData, [DefaultParameterValue(null)] ref CursorPosition cursorPosition, [DefaultParameterValue(null)] ref EntityManager entityManager)

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate void UpdateCameraHandler(
        IntPtr _this,
        ref CameraTarget cameraTarget,
        ref TopdownCamera topdownCamera,
        ref TopdownCameraState cameraState,
        ref Translation cameraTranslation,
        ref Rotation cameraRotation
    );
    static UpdateCameraHandler? _updateCameraOriginal;
    static INativeDetour? _updateCameraDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        unsafe delegate void CursorPositionExecuteHandler(
        IntPtr _this,
        ref CollisionWorld collisionWorld,
        ref int heightLevel,
        ref FadeTargetsSingleton fadeTargets,
        ref CurrentFadingDataSingleton fadeData,
        ref CursorPosition cursorPosition,
        ref EntityManager entityManager
    );

    static CursorPositionExecuteHandler? _cursorPositionExecuteOriginal;
    static INativeDetour? _cursorPositionExecuteDetour;

    // public static CursorPosition _cursorPosition;

    static ZoomSettings _defaultZoomSettings;
    static ZoomSettings _defaultStandardZoomSettings;
    static ZoomSettings _defaultBuildModeZoomSettings;

    static bool _defaultZoomSettingsSaved;
    static bool _usingDefaultZoomSettings;
    public static unsafe void Initialize()
    {
        try
        {
            _handleInputDetour = NativeDetour.Create(typeof(TopdownCameraSystem), "HandleInput", HandleInputPatch, out _handleInputOriginal);
        }
        catch (Exception e)
        {
            Core.Log.LogError($"Failed to create HandleInput detour: {e}");
        }

        try
        {
            _updateCameraDetour = NativeDetour.Create(
            typeof(TopdownCameraSystem),
            "CameraUpdateJob",
            "UpdateCamera",
            UpdateCameraPatch,
            out _updateCameraOriginal
            );
        }
        catch (Exception e)
        {
            Core.Log.LogError($"Failed to create UpdateCamera detour: {e}");
        }

        try
        {
            _cursorPositionExecuteDetour = NativeDetour.Create(
            typeof(CursorPositionSystem),                                   // The containing type
            "CursorPositionSystem_59A0B5A3_LambdaJob_0_Execute",            // The method name
            CursorPositionExecutePatch,         // Our patch method
            out _cursorPositionExecuteOriginal
            );
        }
        catch (Exception e)
        {
            Core.Log.LogError($"Failed to create CursorPositionExecute detour: {e}");
        }
    }
    static unsafe void HandleInputPatch(IntPtr _this, ref InputState inputState)
    {
        if (Settings.Enabled)
        {
            CurrentCameraBehaviour?.HandleInput(ref inputState);
        }

        _handleInputOriginal!(_this, ref inputState);
    }
    static unsafe void UpdateCameraPatch(
        IntPtr _this,
        ref CameraTarget cameraTarget,
        ref TopdownCamera topdownCamera,
        ref TopdownCameraState cameraState,
        ref Translation cameraTranslation,
        ref Rotation cameraRotation
    )
    {
        if (Settings.Enabled)
        {
            // Save default zoom settings once.
            if (!_defaultZoomSettingsSaved)
            {
                _defaultZoomSettings = cameraState.ZoomSettings;
                _defaultStandardZoomSettings = topdownCamera.StandardZoomSettings;
                _defaultBuildModeZoomSettings = topdownCamera.BuildModeZoomSettings;
                _defaultZoomSettingsSaved = true;
            }

            _usingDefaultZoomSettings = false;

            // Override the zoom if your mod wants to do so:
            cameraState.ZoomSettings.MaxZoom = Settings.MaxZoom;
            cameraState.ZoomSettings.MinZoom = 0f;

            // Check camera behaviors for activation
            foreach (CameraBehaviour cameraBehaviour in _cameraBehaviours.Values)
            {
                if (cameraBehaviour.ShouldActivate(ref cameraState))
                {
                    CurrentCameraBehaviour?.Deactivate();
                    cameraBehaviour.Activate(ref cameraState);
                    break;
                }
            }

            // Make sure the current behavior is active
            if (!CurrentCameraBehaviour!.Active)
            {
                CurrentCameraBehaviour!.Activate(ref cameraState);
            }

            // Let your behavior update the camera data
            CurrentCameraBehaviour!.UpdateCameraInputs(ref cameraState, ref topdownCamera);

            // If you need to copy the final ZoomSettings back to the TopdownCamera
            topdownCamera.StandardZoomSettings = cameraState.ZoomSettings;
        }
        else if (_defaultZoomSettingsSaved && !_usingDefaultZoomSettings)
        {
            // Revert to default settings if your mod is disabled
            cameraState.ZoomSettings = _defaultZoomSettings;
            topdownCamera.StandardZoomSettings = _defaultStandardZoomSettings;
            topdownCamera.BuildModeZoomSettings = _defaultBuildModeZoomSettings;
            _usingDefaultZoomSettings = true;
        }

        // Finally, call the original method
        _updateCameraOriginal!(
            _this,
            ref cameraTarget,
            ref topdownCamera,
            ref cameraState,
            ref cameraTranslation,
            ref cameraRotation
        );
    }
    static unsafe void CursorPositionExecutePatch(
    IntPtr _this,
    ref CollisionWorld collisionWorld,
    ref int heightLevel,
    ref FadeTargetsSingleton fadeTargets,
    ref CurrentFadingDataSingleton fadeData,
    ref CursorPosition cursorPosition,
    ref EntityManager entityManager
)
    {
        // Record the cursor position in a static property or field
        // _cursorPosition = cursorPosition;

        // Locks the mouse to the center of the screen if the mouse should be locked or the camera rotate button is pressed
        if (_validGameplayInputState &&
           (_isMouseLocked || _gameplayInputState.IsInputPressed(ButtonInputAction.RotateCamera)) &&
           !IsMenuOpen)
        {
            if (_isActionMode || _isFirstPerson || Settings.CameraAimMode == CameraAimMode.Forward)
            {
                // CursorPosition cursorPosition = _cursorPositionSystem._CursorPosition;

                float2 screenPosition = new((Screen.width / 2) + Settings.AimOffsetX, (Screen.height / 2) - Settings.AimOffsetY);
                cursorPosition.ScreenPosition = screenPosition;

                // _cursorPositionSystem._CursorPosition = cursorPosition;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        // Call the original method so the game logic proceeds
        _cursorPositionExecuteOriginal!(
            _this,
            ref collisionWorld,
            ref heightLevel,
            ref fadeTargets,
            ref fadeData,
            ref cursorPosition,
            ref entityManager
        );
    }
    public static void Dispose()
    {
        _handleInputDetour?.Dispose();
        _updateCameraDetour?.Dispose();
        _cursorPositionExecuteDetour?.Dispose();
    }
}
