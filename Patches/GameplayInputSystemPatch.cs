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
        CameraState._validGameplayInputState = true;
        CameraState._gameplayInputState = inputState;

        // Core.Log.LogWarning($"[GameplayInputSystem.HandleInput]");

        if (!Settings.Enabled) return;
        else if (CameraState._isMouseLocked && !CameraState.IsMenuOpen && !inputState.IsInputPressed(ButtonInputAction.RotateCamera))
        {
            inputState.InputsPressed.m_ListData->AddNoResize(ButtonInputAction.RotateCamera);
        }
    }
}
