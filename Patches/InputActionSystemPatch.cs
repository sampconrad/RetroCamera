using HarmonyLib;
using Il2CppSystem;
using ModernCamera.Configuration;
using ProjectM;
using UnityEngine.InputSystem;
using static ProjectM.InputActionSystem;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class InputActionSystemPatch
{
    static InputActionMap InputActionMap;
    static InputAction ActionModeInputAction;

    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnCreate))]
    [HarmonyPostfix]
    static void OnCreatePostfix(InputActionSystem __instance)
    {
        __instance._LoadedInputActions.Disable();

        foreach (KeybindingCategory keybindingCategory in KeybindingManager.KeybindingCategories.Values)
        {
            __instance._LoadedInputActions.AddActionMap(keybindingCategory.InputActionMap);
        }

        __instance._LoadedInputActions.Enable();
    }

    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix()
    {
        foreach (KeybindingCategory keybindingCategory in KeybindingManager.KeybindingCategories.Values)
        {
            foreach (Keybinding keybinding in keybindingCategory.KeybindingMap.Values)
            {
                if (keybinding.IsDown) keybinding.OnKeyDown();
                if (keybinding.IsPressed) keybinding.OnKeyPressed();
                if (keybinding.IsUp) keybinding.OnKeyUp();
            }
        }
    }
    
    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.ModifyInputActionBinding), typeof(ButtonInputAction), typeof(bool), typeof(Il2CppSystem.Action<bool>), typeof(Il2CppSystem.Action<bool, bool>), typeof(OnRebindCollision), typeof(Nullable_Unboxed<ControllerType>))]
    [HarmonyPrefix]
    static void ModifyInputActionBindingPrefix(ButtonInputAction buttonInput, bool modifyPrimary, ref Il2CppSystem.Action<bool> onComplete, ref Il2CppSystem.Action<bool, bool> onCancel, OnRebindCollision onCollision, Nullable_Unboxed<ControllerType> overrideControllerType)
    {
        Keybinding keybinding = KeybindingManager.GetKeybinding(buttonInput);
        if (keybinding == null) return;

        Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> rebindOnComplete = new(onComplete.Target, onComplete.method);
        Il2CppSystem.Action<InputActionRebindingExtensions.RebindingOperation> rebindOnCancel = new(onCancel.Target, onCancel.method);

        keybinding.StartRebinding(modifyPrimary, rebindOnComplete, rebindOnCancel);

        KeybindingManager.SaveKeybindingCategories();
        KeybindingManager.SaveKeybindings();
    }
}
