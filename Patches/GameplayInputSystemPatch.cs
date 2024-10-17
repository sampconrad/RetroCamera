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
        StateUtilities.ValidGameplayInputState = true;
        StateUtilities.GameplayInputState = inputState;

        if (!Settings.Enabled) return;
        else if (StateUtilities.IsMouseLocked && !StateUtilities.IsMenuOpen && !inputState.IsInputPressed(ButtonInputAction.RotateCamera))
        {
            inputState.InputsPressed.m_ListData->AddNoResize(ButtonInputAction.RotateCamera);
        }
    }
}
