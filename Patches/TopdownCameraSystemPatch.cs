using BepInEx.Unity.IL2CPP.Hook;
using ModernCamera.Behaviours;
using ProjectM;
using System.Runtime.InteropServices;
using static ModernCamera.Utilities.CameraStateUtilities;

namespace ModernCamera.Patches;
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

    static readonly string HandleInputPtrString = "7FFA8331F8C0";
    static readonly string UpdateCameraInputsPtrString = "7FFA83331E10";
    public static unsafe void Initialize()
    {
        HandleInputDetour = INativeDetour.CreateAndApply((IntPtr)Convert.ToInt64(HandleInputPtrString, 16), HandleInputPatch, out HandleInputOriginal);
        UpdateCameraInputsDetour = INativeDetour.CreateAndApply((IntPtr)Convert.ToInt64(UpdateCameraInputsPtrString, 16), UpdateCameraInputsPatch, out UpdateCameraInputsOriginal);
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
