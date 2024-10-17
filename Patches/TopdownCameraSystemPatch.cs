using BepInEx.Unity.IL2CPP.Hook;
using HarmonyLib;
using ModernCamera.Behaviours;
using ProjectM;
using System.Runtime.InteropServices;
using static ModernCamera.Utilities.InteropUtilities;
using static ModernCamera.Utilities.CameraStateUtilities;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class TopdownCameraSystemPatch
{
    static ZoomSettings DefaultZoomSettings;
    static ZoomSettings DefaultStandardZoomSettings;
    static ZoomSettings DefaultBuildModeZoomSettings;

    static bool DefaultZoomSettingsSaved;
    static bool UsingDefaultZoomSettings;

    // Delegates and detour handles
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void HandleInputDelegate(IntPtr _this, ref InputState inputState);
    static HandleInputDelegate HandleInputOriginal;
    static INativeDetour HandleInputDetour;
    const int HandleInputToken = 100663443;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    unsafe delegate void UpdateCameraInputsDelegate(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData);
    static UpdateCameraInputsDelegate UpdateCameraInputsOriginal;
    static INativeDetour UpdateCameraInputsDetour;
    const int UpdateCameraInputsToken = 100663445;
    public static unsafe void Initialize()
    {
        HandleInputDetour = Detour(typeof(TopdownCameraSystem), HandleInputToken, HandleInputPatch, out HandleInputOriginal);
        UpdateCameraInputsDetour = Detour(typeof(TopdownCameraSystem), UpdateCameraInputsToken, UpdateCameraInputsPatch, out UpdateCameraInputsOriginal);
    }
    static unsafe void HandleInputPatch(IntPtr _this, ref InputState inputState)
    {
        if (Settings.Enabled)
        {
            Core.Log.LogInfo("Handling inputs via detour...");

            CurrentCameraBehaviour?.HandleInput(ref inputState);
        }

        HandleInputOriginal!(_this, ref inputState);
    }
    static unsafe void UpdateCameraInputsPatch(IntPtr _this, ref TopdownCameraState cameraState, ref TopdownCamera cameraData)
    {
        if (Settings.Enabled)
        {
            Core.Log.LogInfo("Updating camera inputs via detour...");

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
            if (!CurrentCameraBehaviour!.Active) CurrentCameraBehaviour?.Activate(ref cameraState);

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

    [HarmonyPatch(typeof(TopdownCameraSystem), nameof(TopdownCameraSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix(TopdownCameraSystem __instance)
    {
        if (Settings.Enabled) __instance._ZoomModifierSystem._ZoomModifiers.Clear();
    }
}
