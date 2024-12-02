using HarmonyLib;
using RetroCamera.Utilities;
using ProjectM;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class GameplayInputSystemPatch
{
    [HarmonyPatch(typeof(GameplayInputSystem), nameof(GameplayInputSystem.HandleInput))]
    [HarmonyPrefix]
    static unsafe void HandleInputPrefix(ref InputState inputState)
    {
        CameraState.ValidGameplayInputState = true;
        CameraState.GameplayInputState = inputState;

        if (!Settings.Enabled) return;
        else if (CameraState.IsMouseLocked && !CameraState.IsMenuOpen && !inputState.IsInputPressed(ButtonInputAction.RotateCamera))
        {
            inputState.InputsPressed.m_ListData->AddNoResize(ButtonInputAction.RotateCamera);
        }
    }
}
