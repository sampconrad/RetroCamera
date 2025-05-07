using HarmonyLib;
using ProjectM.UI;
using RetroCamera.Utilities;
using System.Collections;
using UnityEngine;
using static RetroCamera.Utilities.CameraState;

namespace RetroCamera.Patches;

[HarmonyPatch]
internal static class EscapeMenuViewPatch
{
    static readonly WaitForSeconds _resumeDelay = new(0.25f);

    public static bool _isEscapeMenuOpen = false;
    public static bool _isServerPaused = false;
    static bool _wasEnabled = false;
    static bool _wasActionMode = false;

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_Pause))]
    [HarmonyPrefix]
    static void OnPausePrefix()
    {
        // Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Prefix");

        _isServerPaused = !_isServerPaused;

        if (_isServerPaused)
        {
            // Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Paused!");
            _wasEnabled = Settings.Enabled;
            _wasActionMode = CameraState._isActionMode;
            Settings.Enabled = false;
        }

        /*
        else if (_wasEnabled)
        {
            Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Unpaused!");
            Settings.Enabled = _wasEnabled;
            _wasEnabled = false;
        }
        */
    }

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_Pause))]
    [HarmonyPostfix]
    static void OnPausePostfix()
    {
        // Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Postfix");

        // _isServerPaused = !_isServerPaused;

        /*
        if (_isServerPaused)
        {
            Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Paused!");
            _wasEnabled = Settings.Enabled;
            Settings.Enabled = false;
        }

        if (_wasEnabled && !_isServerPaused)
        {
            Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Unpaused!");
            Settings.Enabled = _wasEnabled;
            _wasEnabled = false;
        }
        */

        OnResumeRoutine().Start();
    }
    static IEnumerator OnResumeRoutine()
    {
        yield return _resumeDelay;

        if (_isServerPaused) yield break;
        else if (_wasEnabled && !_isServerPaused)
        {
            // Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Unpaused!");
            Settings.Enabled = _wasEnabled;

            if (_wasActionMode && !_isFirstPerson)
            {
                Settings._wasDisabled = !_wasEnabled;
                Settings.Enabled = true;

                _isMouseLocked = !_isMouseLocked;
                _isActionMode = !_isActionMode;

                /*
                if (IsMenuOpen) IsMenuOpen = false;
                if (ActionWheelSystemPatch._wheelVisible) ActionWheelSystemPatch._wheelVisible = false;
                if (Cursor.lockState.Equals(CursorLockMode.Locked) && (!_isActionMode || !_isMouseLocked))
                {
                    Cursor.lockState = CursorLockMode.None;
                }
                */
            }
            else if (_wasEnabled)
            {
                if (IsMenuOpen)
                {
                    // Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_Pause] Unpaused! Resetting IsMenuOpen");
                    IsMenuOpen = false;
                }
            }

            _wasEnabled = false;
        }
    }

    /*
    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnButtonClick_ResumeGame))]
    [HarmonyPostfix]
    static void OnResumePostfix()
    {
        Core.Log.LogWarning($"[EscapeMenuView.OnButtonClick_ResumeGame] Resumed!");
        _isServerPaused = false;
        if (_wasEnabled) Settings.Enabled = true;
    }
    */

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnEnable))]
    [HarmonyPostfix]
    static void OnEnablePostfix() => Enable();

    [HarmonyPatch(typeof(EscapeMenuView), nameof(EscapeMenuView.OnDestroy))]
    [HarmonyPostfix]
    static void OnDestroyPostfix() => Disable();
    static void Enable()
    {
        if (!_isEscapeMenuOpen)
        {
            _isEscapeMenuOpen = true;
            IsMenuOpen = true;
        }
    }
    static void Disable()
    {
        if (_isEscapeMenuOpen)
        {
            _isEscapeMenuOpen = false;
            IsMenuOpen = false;
        }
    }
}
