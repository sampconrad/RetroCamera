﻿using HarmonyLib;
using ProjectM.UI;
using RetroCamera.Configuration;
using RetroCamera.Utilities;
using static RetroCamera.Configuration.QuipManager;
using static RetroCamera.Utilities.Quips;
using static RetroCamera.Systems.RetroCamera;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class ActionWheelSystemPatch
{
    public static bool _wheelVisible = false;

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.OnUpdate))]
    [HarmonyPostfix]
    static void OnUpdatePostfix(ActionWheelSystem __instance)
    {
        if (_wheelVisible)
        {
            if (__instance._CurrentActiveWheel != null && !__instance._CurrentActiveWheel.IsVisible())
            {
                CameraState.IsMenuOpen = false;
                _wheelVisible = false;
            }
            else if (__instance._CurrentActiveWheel == null)
            {
                CameraState.IsMenuOpen = false;
                _wheelVisible = false;
            }
        }
        else if (__instance._CurrentActiveWheel != null && __instance._CurrentActiveWheel.IsVisible())
        {
            _wheelVisible = true;
            CameraState.IsMenuOpen = true;
        }

        // Core.Log.LogWarning($"IsWheelActive {ActionWheelSystem.IsWheelActive}");
    }

    static DateTime _wheelOpened = DateTime.MinValue;
    static DateTime _lastQuipSendTime = DateTime.MinValue;
    const float QUIP_COOLDOWN_SECONDS = 0.5f;

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.SendQuipChatMessage))]
    [HarmonyPrefix]
    static bool SendQuipChatMessagePrefix(byte index)
    {
        DateTime now = DateTime.UtcNow;

        if (_wheelOpened.Equals(DateTime.MinValue))
        {
            _wheelOpened = now;
        }

        if ((now - _wheelOpened).TotalSeconds < 0.1f)
            return false;

        if ((now - _lastQuipSendTime).TotalSeconds < QUIP_COOLDOWN_SECONDS)
            return false;

        _lastQuipSendTime = now;

        if (CommandQuips.TryGetValue(index, out CommandQuip commandQuip))
        {
            SendCommandQuip(commandQuip);
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.HideCurrentWheel))]
    [HarmonyPrefix]
    static bool HideCurrentWheelPrefix(ActionWheelSystem __instance)
    {
        if (SocialWheelActive)
        {
            return false;
        }
        else if (!_wheelOpened.Equals(DateTime.MinValue))
        {
            _wheelOpened = DateTime.MinValue;
        }

        // Core.Log.LogWarning($"[ActionWheelSystem.HideCurrentWheel]");
        return true;
    }

    [HarmonyPatch(typeof(ActionWheelSystem), nameof(ActionWheelSystem.UpdateAndShowWheel))]
    [HarmonyPrefix]
    static void UpdateAndShowWheelPrefix(ActionWheelSystem __instance)
    {
        // Core.Log.LogWarning($"[ActionWheelSystem.UpdateAndShowWheel]");
    }
}
