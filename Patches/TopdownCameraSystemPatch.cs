using BepInEx.Unity.IL2CPP.Hook;
using ProjectM;
using RetroCamera.Behaviours;
using RetroCamera.Utilities;
using System.Runtime.InteropServices;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Patches;
#nullable enable
internal static class TopdownCameraSystemPatch
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void HandleInputHandler(IntPtr _this, ref InputState inputState);
    static HandleInputHandler? _handleInputOriginal;
    static INativeDetour? _handleInputDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void UpdateCameraInputsHandler(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData);
    static UpdateCameraInputsHandler? _updateCameraInputsOriginal;
    static INativeDetour? _updateCameraInputsDetour;

    static ZoomSettings _defaultZoomSettings;
    static ZoomSettings _defaultStandardZoomSettings;
    static ZoomSettings _defaultBuildModeZoomSettings;

    static bool _defaultZoomSettingsSaved;
    static bool _usingDefaultZoomSettings;

    public static unsafe void Initialize()
    {
        //HandleInputDetour = HandleInputDetour(HandleInputToken, HandleInputPatch, out HandleInputOriginal);
        //UpdateCameraInputsDetour = UpdateCameraInputsDetour(UpdateCameraInputsToken, UpdateCameraInputsPatch, out UpdateCameraInputsOriginal);

        _handleInputDetour = Detour.Create(typeof(TopdownCameraSystem), "HandleInput", HandleInputPatch, out _handleInputOriginal);
        _updateCameraInputsDetour = Detour.Create(typeof(TopdownCameraSystem), "UpdateCameraInputs", "OriginalLambdaBody", UpdateCameraInputsPatch, out _updateCameraInputsOriginal);
    }
    static unsafe void HandleInputPatch(IntPtr _this, ref InputState inputState)
    {
        if (Settings.Enabled)
        {
            CurrentCameraBehaviour?.HandleInput(ref inputState);
        }

        _handleInputOriginal!(_this, ref inputState);
    }
    static unsafe void UpdateCameraInputsPatch(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData)
    {
        if (Settings.Enabled)
        {
            if (!_defaultZoomSettingsSaved)
            {
                _defaultZoomSettings = cameraState.ZoomSettings;
                _defaultStandardZoomSettings = cameraData.StandardZoomSettings;
                _defaultBuildModeZoomSettings = cameraData.BuildModeZoomSettings;

                _defaultZoomSettingsSaved = true;
            }

            _usingDefaultZoomSettings = false;

            // Set zoom settings
            cameraState.ZoomSettings.MaxZoom = Settings.MaxZoom;
            cameraState.ZoomSettings.MinZoom = 0f;

            // Check camera behaviours for activation
            foreach (CameraBehaviour cameraBehaviour in _cameraBehaviours.Values)
            {
                if (cameraBehaviour.ShouldActivate(ref cameraState))
                {
                    CurrentCameraBehaviour?.Deactivate();
                    cameraBehaviour.Activate(ref cameraState);

                    break;
                }
            }

            // Update current camera behaviour
            if (!CurrentCameraBehaviour!.Active) CurrentCameraBehaviour!.Activate(ref cameraState);

            CurrentCameraBehaviour!.UpdateCameraInputs(ref cameraState, ref cameraData);
            cameraData.StandardZoomSettings = cameraState.ZoomSettings;
        }
        else if (_defaultZoomSettingsSaved && !_usingDefaultZoomSettings)
        {
            cameraState.ZoomSettings = _defaultZoomSettings;

            cameraData.StandardZoomSettings = _defaultStandardZoomSettings;
            cameraData.BuildModeZoomSettings = _defaultBuildModeZoomSettings;

            _usingDefaultZoomSettings = true;
        }

        _updateCameraInputsOriginal!(_this, ref cameraState, ref cameraData);
    }
    public static void Dispose()
    {
        _handleInputDetour?.Dispose();
        _updateCameraInputsDetour?.Dispose();
    }
}
