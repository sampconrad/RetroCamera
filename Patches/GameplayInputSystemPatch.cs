using HarmonyLib;
using ModernCamera.Utilities;
using ProjectM;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class GameplayInputSystemPatch
{
    [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
    [HarmonyPrefix]
    static unsafe void HandleInputPrefix(ref InputState inputState)
    {
        CameraStateUtilities.ValidGameplayInputState = true;
        CameraStateUtilities.GameplayInputState = inputState;

        if (!Settings.Enabled) return;
        else if (CameraStateUtilities.IsMouseLocked && !CameraStateUtilities.IsMenuOpen && !inputState.IsInputPressed(ButtonInputAction.RotateCamera))
        {
            inputState.InputsPressed.m_ListData->AddNoResize(ButtonInputAction.RotateCamera);
        }
    }
}
