﻿using HarmonyLib;
using ProjectM;
using RetroCamera.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RetroCamera.Patches;

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

        InputActionMap inputActionMap = new(LocalizationManager.HEADER);
        __instance._LoadedInputActions.m_ActionMaps.AddItem(inputActionMap);

        __instance._LoadedInputActions.Enable();
    }

    [HarmonyPatch(typeof(InputActionSystem), nameof(InputActionSystem.OnUpdate))]
    [HarmonyPrefix]
    static void OnUpdatePrefix()
    {
        foreach (Keybinding keybind in KeybindsManager.Keybinds.Values)
        {
            if (IsKeybindDown(keybind)) keybind.KeyDown();
            if (IsKeybindUp(keybind)) keybind.KeyUp();
            if (IsKeybindPressed(keybind)) keybind.KeyPressed();
        }
    }
    static bool IsKeybindDown(Keybinding keybind)
    {
        return Input.GetKeyDown(keybind.Primary);
    }
    static bool IsKeybindUp(Keybinding keybind)
    {
        return Input.GetKeyUp(keybind.Primary);
    }
    static bool IsKeybindPressed(Keybinding keybind)
    {
        return Input.GetKey(keybind.Primary);
    }
}
