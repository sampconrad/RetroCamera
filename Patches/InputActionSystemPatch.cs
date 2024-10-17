using HarmonyLib;
using ModernCamera.Configuration;
using ProjectM;
using UnityEngine.InputSystem;
using static ModernCamera.Configuration.KeybindActions;
using static ModernCamera.Configuration.KeybindCategories;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class InputActionSystemPatch
{
    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnCreate))]
    [HarmonyPostfix]
    static void OnCreatePostfix(InputActionSystem __instance)
    {
        __instance._LoadedInputActions.Disable();

        foreach (KeybindCategory keybindingCategory in KeybindsManager.KeybindCategories.Values)
        {
            __instance._LoadedInputActions.AddActionMap(keybindingCategory.InputActionMap);
        }

        __instance._LoadedInputActions.Enable();
    }

    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix()
    {
        foreach (KeybindMapping keybind in KeybindsManager.KeybindsById.Values)
        {
            if (keybind.IsDown) keybind.OnKeyDown();
            if (keybind.IsPressed) keybind.OnKeyPressed();
            if (keybind.IsUp) keybind.OnKeyUp();
        }
    }
    
    /*
    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.ModifyInputActionBinding), typeof(ButtonInputAction), typeof(bool), typeof(Il2CppSystem.Action<bool>), typeof(Il2CppSystem.Action<bool, bool>), typeof(OnRebindCollision), typeof(Nullable_Unboxed<ControllerType>))]
    [HarmonyPrefix]
    static void ModifyInputActionBindingPrefix(ButtonInputAction buttonInput, bool modifyPrimary, ref Il2CppSystem.Action<bool> onComplete, ref Il2CppSystem.Action<bool, bool> onCancel, OnRebindCollision onCollision, Nullable_Unboxed<ControllerType> overrideControllerType)
    {
        KeybindMapping keybinding = KeybindsManager.GetKeybinding(buttonInput);
        if (keybinding == null) return;

        Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> rebindOnComplete = new(onComplete.Target, onComplete.method);
        Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> rebindOnCancel = new(onCancel.Target, onCancel.method);

        keybinding.StartRebinding(modifyPrimary, rebindOnComplete, rebindOnCancel);

        KeybindsManager.SaveKeybindingCategories();
        KeybindsManager.SaveKeybindings();
    }
    */
}
