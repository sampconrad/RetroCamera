using HarmonyLib;
using ModernCamera.Configuration;
using ProjectM;
using UnityEngine.InputSystem;
using UnityEngine;
using static ModernCamera.Configuration.KeybindCategories;
using static ModernCamera.Configuration.KeybindCategories.KeybindCategory;

namespace ModernCamera.Patches;

[HarmonyPatch]
internal static class InputActionSystemPatch
{
    [HarmonyPatch(typeof(TopdownCameraSystem), nameof(TopdownCameraSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix(TopdownCameraSystem __instance)
    {
        if (Settings.Enabled) __instance._ZoomModifierSystem._ZoomModifiers.Clear();
    }

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
            if (IsKeybindDown(keybind)) keybind.OnKeyDown();
            if (IsKeybindUp(keybind)) keybind.OnKeyUp();
            if (IsKeybindPressed(keybind)) keybind.OnKeyPressed();
        }
    }
    static bool IsKeybindDown(KeybindMapping keybind)
    {
        return Input.GetKey(keybind.Primary) || Input.GetKey(keybind.Secondary);
    }
    static bool IsKeybindUp(KeybindMapping keybind)
    {
        return Input.GetKeyUp(keybind.Primary) || Input.GetKeyUp(keybind.Secondary);
    }
    static bool IsKeybindPressed(KeybindMapping keybind)
    {
        return Input.GetKeyDown(keybind.Primary) || Input.GetKeyDown(keybind.Secondary);
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
