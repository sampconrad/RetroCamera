using ProjectM;
using RetroCamera.Behaviours;
using UnityEngine;

namespace RetroCamera.Utilities;
internal static class CameraState
{
    public static bool _isUIHidden;
    public static bool _isFirstPerson;
    public static bool _isActionMode;
    public static bool _isMouseLocked;
    public static bool _isShapeshifted;
    public static bool _isMounted;
    public static bool _inBuildMode;
    public static bool _validGameplayInputState;

    public static BehaviourType _currentBehaviourType = BehaviourType.Default;
    public static Dictionary<BehaviourType, CameraBehaviour> _cameraBehaviours = [];
    public static InputState _gameplayInputState;

    public static string _shapeshiftName;
    static int _menusOpen;
    public enum BehaviourType
    {
        Default,
        FirstPerson,
        ThirdPerson
    }
    public enum CameraAimMode
    {
        Default,
        Forward
    }
    public static bool IsMenuOpen
    {
        get { return _menusOpen > 0; }
        set { _menusOpen = value ? _menusOpen + 1 : Mathf.Max(0, _menusOpen - 1); }
    }
    public static CameraBehaviour CurrentCameraBehaviour
    {
        get
        {
            if (_cameraBehaviours.ContainsKey(_currentBehaviourType)) return _cameraBehaviours[_currentBehaviourType];
            else return null;
        }
    }
    public static void RegisterCameraBehaviour(CameraBehaviour behaviour)
    {
        _cameraBehaviours.Add(behaviour.BehaviourType, behaviour);
    }
    public static void Reset()
    {
        _isUIHidden = false;
        _isFirstPerson = false;
        _isActionMode = false;
        _isMouseLocked = false;
        _isShapeshifted = false;
        _isMounted = false;
        _inBuildMode = false;
        _shapeshiftName = "";
        _validGameplayInputState = false;

        CurrentCameraBehaviour?.Deactivate();
        _currentBehaviourType = BehaviourType.Default;
    }
}
