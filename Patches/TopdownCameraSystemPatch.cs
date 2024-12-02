using BepInEx.Unity.IL2CPP.Hook;
using RetroCamera.Behaviours;
using ProjectM;
using System.Runtime.InteropServices;
using static RetroCamera.Utilities.CameraState;
using static RetroCamera.Utilities.NativeDetour;
using RetroCamera.Utilities;

namespace RetroCamera.Patches;
#nullable enable
internal static class TopdownCameraSystemPatch
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void HandleInput(IntPtr _this, ref InputState inputState);
    static HandleInput? HandleInputOriginal;
    static INativeDetour? HandleInputDetour;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void UpdateCameraInputs(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData);
    static UpdateCameraInputs? UpdateCameraInputsOriginal;
    static INativeDetour? UpdateCameraInputsDetour;

    static ZoomSettings DefaultZoomSettings;
    static ZoomSettings DefaultStandardZoomSettings;
    static ZoomSettings DefaultBuildModeZoomSettings;

    static bool DefaultZoomSettingsSaved;
    static bool UsingDefaultZoomSettings;

    public static unsafe void Initialize()
    {
        //HandleInputDetour = HandleInputDetour(HandleInputToken, HandleInputPatch, out HandleInputOriginal);
        //UpdateCameraInputsDetour = UpdateCameraInputsDetour(UpdateCameraInputsToken, UpdateCameraInputsPatch, out UpdateCameraInputsOriginal);

        HandleInputDetour = NativeDetour.Create(typeof(TopdownCameraSystem), "HandleInput", HandleInputPatch, out HandleInputOriginal);
        UpdateCameraInputsDetour = NativeDetour.Create(typeof(TopdownCameraSystem), "UpdateCameraInputs", "OriginalLambdaBody", UpdateCameraInputsPatch, out UpdateCameraInputsOriginal);
    }
    static unsafe void HandleInputPatch(IntPtr _this, ref InputState inputState)
    {
        if (Settings.Enabled)
        {
            CurrentCameraBehaviour?.HandleInput(ref inputState);
        }

        HandleInputOriginal!(_this, ref inputState);
    }
    static unsafe void UpdateCameraInputsPatch(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData)
    {
        if (Settings.Enabled)
        {
            if (!DefaultZoomSettingsSaved)
            {
                DefaultZoomSettings = cameraState.ZoomSettings;
                DefaultStandardZoomSettings = cameraData.StandardZoomSettings;
                DefaultBuildModeZoomSettings = cameraData.BuildModeZoomSettings;

                DefaultZoomSettingsSaved = true;
            }

            UsingDefaultZoomSettings = false;

            // Set zoom settings
            cameraState.ZoomSettings.MaxZoom = Settings.MaxZoom;
            cameraState.ZoomSettings.MinZoom = 0f;

            // Check camera behaviours for activation
            foreach (CameraBehaviour cameraBehaviour in CameraBehaviours.Values)
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
        else if (DefaultZoomSettingsSaved && !UsingDefaultZoomSettings)
        {
            cameraState.ZoomSettings = DefaultZoomSettings;

            cameraData.StandardZoomSettings = DefaultStandardZoomSettings;
            cameraData.BuildModeZoomSettings = DefaultBuildModeZoomSettings;

            UsingDefaultZoomSettings = true;
        }

        UpdateCameraInputsOriginal!(_this, ref cameraState, ref cameraData);
    }
    public static void Dispose()
    {
        HandleInputDetour?.Dispose();
        UpdateCameraInputsDetour?.Dispose();
    }
}
